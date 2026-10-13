using System;
using System.Collections.Generic;
using CodeBoost.CodeAnalysis;
using CodeBoost.Extensions;
using CodeBoost.Performance;

namespace CodeBoost.Networking;

/// <summary>
/// Rebuilds payloads a sender split into numbered segments, holding each payload's segments until all of them have arrived and handing
/// back the whole payload once they have.
/// </summary>
/// <remarks>
/// <para>
/// Shared by every networking layer that splits a payload too large for one datagram. Each layer writes and reads its own segment header
/// and passes the fields it read here: nothing in this type knows a wire format or a clock, so the caller supplies the segment fields and
/// the time alike, in whatever tick unit it uses consistently.
/// </para>
/// <para>
/// Rent it through <see cref="ResettableObjectPool{T0}"/>, call <see cref="Initialize"/>, and return it to the pool when done, which
/// releases every assembly still held.
/// </para>
/// </remarks>
public sealed class PacketReassembler : IPoolResettable
{
    /// <summary>
    /// The most segments one payload may be split into. Set by <see cref="Initialize"/>.
    /// </summary>
    [PoolResettableMember]
    public uint MaximumSegments { get; private set; }
    /// <summary>
    /// The most bytes one segment may carry. Set by <see cref="Initialize"/>.
    /// </summary>
    /// <remarks>
    /// Bounds the memory one payload can hold to this times <see cref="MaximumSegments"/>. A segment declaring more is refused, since
    /// otherwise a sender could declare two segments and then send two of any size.
    /// </remarks>
    [PoolResettableMember]
    public uint MaximumSegmentLength { get; private set; }
    /// <summary>
    /// True while at least one payload is waiting for segments.
    /// </summary>
    public bool HasAssemblies
    {
        get
        {
            lock (_lock)
                return _currentSegments.Count > 0;
        }
    }
    /// <summary>
    /// Pass as <c>maximumConcurrentAssemblies</c> to <see cref="Initialize"/> to remove the limit on payloads in progress.
    /// </summary>
    public const uint UnsetMaximumConcurrentAssemblies = 0;
    /// <summary>
    /// The most segments a payload can be declared to have, being what one byte of segment count can say.
    /// </summary>
    public const uint MaximumSegmentCount = byte.MaxValue;

    /// <summary>
    /// The most payloads that may be in progress at once, with <see cref="UnsetMaximumConcurrentAssemblies"/> stored as
    /// <see cref="uint.MaxValue"/> so the check is a single comparison.
    /// </summary>
    [PoolResettableMember]
    private uint _maximumConcurrentAssemblies;
    /// <summary>
    /// The payloads in progress, keyed by segment id.
    /// </summary>
    private readonly Dictionary<ushort, SegmentAssembly> _currentSegments = [];
    /// <summary>
    /// Guards <see cref="_currentSegments"/>.
    /// </summary>
    private readonly object _lock = new();

    /// <summary>
    /// Configures the reassembler after it is rented.
    /// </summary>
    /// <param name="maximumSegmentLength">The most bytes one segment may carry; must be greater than zero.</param>
    /// <param name="maximumSegments">The most segments one payload may be split into, from 1 to <see cref="MaximumSegmentCount"/>.</param>
    /// <param name="maximumConcurrentAssemblies">The most payloads in progress at once, or <see cref="UnsetMaximumConcurrentAssemblies"/> for no limit.</param>
    [PoolResettableMethod]
    public void Initialize(uint maximumSegmentLength, uint maximumSegments, uint maximumConcurrentAssemblies)
    {
        if (maximumSegmentLength == 0)
            throw new ArgumentOutOfRangeException(nameof(maximumSegmentLength));

        if (maximumSegments is 0 or > MaximumSegmentCount)
            throw new ArgumentOutOfRangeException(nameof(maximumSegments));

        MaximumSegmentLength = maximumSegmentLength;
        MaximumSegments = maximumSegments;
        _maximumConcurrentAssemblies = maximumConcurrentAssemblies == UnsetMaximumConcurrentAssemblies ? uint.MaxValue : maximumConcurrentAssemblies;
    }

    /// <summary>
    /// Adds a received segment, and hands back the whole payload when it completes one.
    /// </summary>
    /// <param name="segmentId">The id every segment of the same payload shares.</param>
    /// <param name="segmentIndex">This segment's zero-based position in the payload.</param>
    /// <param name="segmentCount">How many segments the payload has.</param>
    /// <param name="segmentData">This segment's bytes, copied before the call returns.</param>
    /// <param name="isReliable">Whether the segment arrived on a reliable channel.</param>
    /// <param name="nowTicks">The current time, in the unit the caller passes to <see cref="RemoveExpiredSegments"/>.</param>
    /// <param name="assembledSegments">The whole payload when the method returns true, rented from <see cref="TrackedArrayPool{T0}"/>; the caller returns its array.</param>
    /// <param name="isProtocolViolation">True when the segment is inconsistent with what the sender declared, which no honest sender produces.</param>
    /// <param name="isDuplicateSegment">True when this segment was already held, so the sender is resending it.</param>
    /// <returns>True when this segment completed its payload.</returns>
    /// <remarks>
    /// <para>
    /// A protocol violation is a segment larger than <see cref="MaximumSegmentLength"/>, or one whose segment count or reliability differs
    /// from the payload it names, which also discards that payload. Callers treat it as grounds to drop the peer.
    /// </para>
    /// <para>
    /// A segment that would open a payload beyond the concurrent limit first evicts the oldest incomplete unreliable one, which is usually
    /// waiting on a segment that was lost and will never be resent. When every payload in progress is reliable, the segment is dropped
    /// instead, without a violation, and the sender's resend brings it back once a slot frees.
    /// </para>
    /// </remarks>
    public bool TryReassemble(ushort segmentId, byte segmentIndex, byte segmentCount, ArraySegment<byte> segmentData, bool isReliable, long nowTicks, out ArraySegment<byte> assembledSegments, out bool isProtocolViolation, out bool isDuplicateSegment)
    {
        assembledSegments = default;
        isProtocolViolation = false;
        isDuplicateSegment = false;

        if (segmentCount == 0 || segmentCount > MaximumSegments || segmentIndex >= segmentCount)
            return false;

        if (segmentData.Count > MaximumSegmentLength)
        {
            isProtocolViolation = true;

            return false;
        }

        lock (_lock)
        {
            if (!_currentSegments.TryGetValue(segmentId, out SegmentAssembly? segmentAssembly))
            {
                // At the cap the segment is dropped, not reported: a reliable one is resent, and an unreliable one may be lost.
                if (_currentSegments.Count >= _maximumConcurrentAssemblies && !TryEvictOldestUnreliableAssembly())
                    return false;

                segmentAssembly = ResettableObjectPool<SegmentAssembly>.Rent();
                segmentAssembly.Initialize(segmentCount, isReliable, nowTicks);
                _currentSegments[segmentId] = segmentAssembly;
            }
            else if (segmentAssembly.SegmentCount != segmentCount || segmentAssembly.IsReliable != isReliable)
            {
                // The same id with a different shape is a sender bug or an attempt to desync this state, so the payload is discarded.
                _currentSegments.Remove(segmentId);
                ResettableObjectPool<SegmentAssembly>.Return(segmentAssembly);
                isProtocolViolation = true;

                return false;
            }

            isDuplicateSegment = !segmentAssembly.Add(segmentIndex, segmentData);

            if (!segmentAssembly.TryGetAssembledSegments(out assembledSegments))
                return false;

            _currentSegments.Remove(segmentId);
            ResettableObjectPool<SegmentAssembly>.Return(segmentAssembly);

            return true;
        }
    }

    /// <summary>
    /// Writes one bit per segment held for a payload in progress, so a sender can resend only the ones missing.
    /// </summary>
    /// <param name="segmentId">The payload's segment id.</param>
    /// <param name="bitmap">Receives the bits, one per segment index; at least <c>(segmentCount + 7) / 8</c> bytes.</param>
    /// <param name="segmentCount">How many segments the payload has.</param>
    /// <returns>False when no payload with that id is in progress.</returns>
    public bool TryWriteReceivedBitmap(ushort segmentId, ArraySegment<byte> bitmap, out byte segmentCount)
    {
        lock (_lock)
        {
            if (!_currentSegments.TryGetValue(segmentId, out SegmentAssembly? segmentAssembly))
            {
                segmentCount = 0;

                return false;
            }

            segmentAssembly.WriteReceivedBitmap(bitmap);
            segmentCount = (byte)segmentAssembly.SegmentCount;

            return true;
        }
    }

    /// <summary>
    /// Discards every payload whose first segment arrived longer ago than <paramref name="timeoutTicks"/>.
    /// </summary>
    /// <param name="nowTicks">The current time, in the unit passed to <see cref="TryReassemble"/>.</param>
    /// <param name="timeoutTicks">How long a payload may wait for its segments.</param>
    public void RemoveExpiredSegments(long nowTicks, long timeoutTicks)
    {
        // Usually called with nothing in progress, so that case skips the lock and the pooled list entirely.
        if (_currentSegments.Count == 0)
            return;

        lock (_lock)
        {
            List<ushort> expiredSegmentIds = ListPool<ushort>.Rent();

            try
            {
                foreach (KeyValuePair<ushort, SegmentAssembly> keyValuePair in _currentSegments)
                {
                    if (nowTicks - keyValuePair.Value.FirstReceivedTicks > timeoutTicks)
                        expiredSegmentIds.Add(keyValuePair.Key);
                }

                foreach (ushort expiredSegmentId in expiredSegmentIds)
                    RemoveAssembly(expiredSegmentId);
            }
            finally
            {
                ListPool<ushort>.Return(expiredSegmentIds);
            }
        }
    }

    /// <summary>
    /// Makes room for a new payload by evicting the oldest incomplete unreliable one. Must be called under <see cref="_lock"/>.
    /// </summary>
    /// <returns>True when a payload was evicted; false when every payload in progress is reliable.</returns>
    /// <remarks>
    /// An incomplete unreliable payload is usually waiting on a segment that was lost and will never be resent, so it would otherwise hold
    /// its slot until it expired, and a few seconds of loss filled the reassembler with them. A reliable payload is never evicted, because
    /// its missing segments are on their way.
    /// </remarks>
    private bool TryEvictOldestUnreliableAssembly()
    {
        SegmentAssembly? oldestAssembly = null;
        ushort oldestSegmentId = 0;

        foreach (KeyValuePair<ushort, SegmentAssembly> keyValuePair in _currentSegments)
        {
            if (keyValuePair.Value.IsReliable || (oldestAssembly is not null && keyValuePair.Value.FirstReceivedTicks >= oldestAssembly.FirstReceivedTicks))
                continue;

            oldestAssembly = keyValuePair.Value;
            oldestSegmentId = keyValuePair.Key;
        }

        if (oldestAssembly is null)
            return false;

        RemoveAssembly(oldestSegmentId);

        return true;
    }

    /// <summary>
    /// Removes one payload in progress and returns it to its pool. Must be called under <see cref="_lock"/>.
    /// </summary>
    /// <param name="segmentId">The payload's segment id.</param>
    private void RemoveAssembly(ushort segmentId)
    {
        if (!_currentSegments.TryGetValue(segmentId, out SegmentAssembly? segmentAssembly))
            return;

        _currentSegments.Remove(segmentId);
        ResettableObjectPool<SegmentAssembly>.Return(segmentAssembly);
    }

    /// <inheritdoc/>
    public void OnReturn()
    {
        lock (_lock)
        {
            foreach (SegmentAssembly segmentAssembly in _currentSegments.Values)
                ResettableObjectPool<SegmentAssembly>.Return(segmentAssembly);

            _currentSegments.Clear();
        }

        MaximumSegmentLength = 0;
        MaximumSegments = 0;
        _maximumConcurrentAssemblies = UnsetMaximumConcurrentAssemblies;
    }

    /// <inheritdoc/>
    public void OnRent() { }

    /// <summary>
    /// One payload in progress: its segments stored by index as they arrive, in any order, until all are held.
    /// </summary>
    private sealed class SegmentAssembly : IPoolResettable
    {
        /// <summary>
        /// How many segments the payload has.
        /// </summary>
        [PoolResettableMember]
        public uint SegmentCount { get; private set; }
        /// <summary>
        /// When the payload's first segment arrived, which is what its expiry is measured from.
        /// </summary>
        [PoolResettableMember]
        public long FirstReceivedTicks;
        /// <summary>
        /// Whether the payload arrives on a reliable channel.
        /// </summary>
        [PoolResettableMember]
        public bool IsReliable;

        /// <summary>
        /// The segments held so far, by index, each in a buffer of its own; an empty slot has a null array.
        /// </summary>
        [PoolResettableMember]
        private List<ArraySegment<byte>>? _segments;
        /// <summary>
        /// How many segments are held.
        /// </summary>
        [PoolResettableMember]
        private uint _receivedCount;
        /// <summary>
        /// The bytes held across all segments.
        /// </summary>
        [PoolResettableMember]
        private uint _totalLength;

        /// <summary>
        /// Prepares the assembly for a payload of the given shape.
        /// </summary>
        /// <param name="segmentCount">How many segments the payload has.</param>
        /// <param name="isReliable">Whether the payload arrives on a reliable channel.</param>
        /// <param name="nowTicks">When its first segment arrived.</param>
        [PoolResettableMethod]
        public void Initialize(byte segmentCount, bool isReliable, long nowTicks)
        {
            SegmentCount = segmentCount;
            IsReliable = isReliable;
            FirstReceivedTicks = nowTicks;

            _segments = ListPool<ArraySegment<byte>>.Rent();

            for (int i = _segments.Count; i < segmentCount; i++)
                _segments.Add(default);
        }

        /// <summary>
        /// Stores a segment at its index, copying its bytes into a pooled buffer.
        /// </summary>
        /// <param name="segmentIndex">The segment's zero-based index.</param>
        /// <param name="segmentData">The segment's bytes.</param>
        /// <returns>False when this index was already held, meaning the sender is resending it.</returns>
        public bool Add(byte segmentIndex, ArraySegment<byte> segmentData)
        {
            if (_segments![segmentIndex].Array is not null)
                return false;

            int segmentLength = segmentData.Count;
            byte[] rentedBuffer = TrackedArrayPool<byte>.Rent(segmentLength);

            if (segmentLength > 0)
                Buffer.BlockCopy(segmentData.Array!, segmentData.Offset, rentedBuffer, 0, segmentLength);

            _segments[segmentIndex] = new(rentedBuffer, offset: 0, segmentLength);
            _receivedCount++;
            _totalLength += (uint)segmentLength;

            return true;
        }

        /// <summary>
        /// Writes one bit per held segment index into <paramref name="bitmap"/>.
        /// </summary>
        /// <param name="bitmap">Receives the bits.</param>
        public void WriteReceivedBitmap(ArraySegment<byte> bitmap)
        {
            Array.Clear(bitmap.Array!, bitmap.Offset, bitmap.Count);

            for (int i = 0; i < SegmentCount && i < _segments!.Count; i++)
            {
                if (_segments[i].Array is not null)
                    bitmap.Array![bitmap.Offset + (i >> 3)] |= (byte)(1 << (i & 7));
            }
        }

        /// <summary>
        /// Joins every segment into one buffer once all are held.
        /// </summary>
        /// <param name="assembledSegment">The whole payload, rented from <see cref="TrackedArrayPool{T0}"/>, when the method returns true.</param>
        /// <returns>True when every segment is held.</returns>
        public bool TryGetAssembledSegments(out ArraySegment<byte> assembledSegment)
        {
            if (_receivedCount != SegmentCount)
            {
                assembledSegment = default;

                return false;
            }

            byte[] reassembledBytes = TrackedArrayPool<byte>.Rent((int)_totalLength);
            int offset = 0;

            for (int i = 0; i < SegmentCount; i++)
            {
                int segmentLength = _segments![i].Count;
                Buffer.BlockCopy(_segments[i].Array!, srcOffset: 0, dst: reassembledBytes, offset, count: segmentLength);
                offset += segmentLength;
            }

            assembledSegment = new(reassembledBytes, offset: 0, (int)_totalLength);

            return true;
        }

        /// <inheritdoc/>
        public void OnReturn()
        {
            if (_segments is not null)
            {
                foreach (ArraySegment<byte> arraySegment in _segments)
                    arraySegment.PoolArrayIntoShared();

                ListPool<ArraySegment<byte>>.ReturnAndNullifyReference(ref _segments);
            }

            SegmentCount = 0;
            FirstReceivedTicks = 0;
            IsReliable = false;
            _receivedCount = 0;
            _totalLength = 0;
        }

        /// <inheritdoc/>
        public void OnRent() { }
    }
}
