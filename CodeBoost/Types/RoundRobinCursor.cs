using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CodeBoost.CodeAnalysis;
using CodeBoost.Logging;
using CodeBoost.Performance;
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8601 // Possible null reference assignment.

namespace CodeBoost.Types;

/// <summary>
/// Walks a read-only list in wrap-around batches, remembering the last returned index across calls.
/// Each call to an Enumerate overload yields the next slice; once the end of the collection is reached the cursor wraps to zero.
/// The batch size scales with the elapsed time and the configured sweep window so that one full pass occurs over that window.
/// </summary>
/// <typeparam name="T0">The element type held by the underlying collection.</typeparam>
public sealed class RoundRobinCursor<T0> : IPoolResettable
{
    /// <summary>
    /// The collection being walked. Held by reference; additions and removals are observed on the next call to Enumerate.
    /// </summary>
    [PoolResettableMember]
    private IReadOnlyList<T0> _collection;
    /// <summary>
    /// The total window, in milliseconds, over which one full pass of the collection should occur.
    /// </summary>
    [PoolResettableMember]
    private uint _sweepWindowMilliseconds;
    /// <summary>
    /// The index of the next element to yield. Wraps to zero when it reaches the collection count.
    /// </summary>
    [PoolResettableMember]
    private uint _lastIndex;
    /// <summary>
    /// The <see cref="DateTime.UtcNow"/> tick value captured by the previous parameterless <see cref="Enumerate()"/> call. Equals <see cref="UnsetLastCallTicks"/> until the first such call completes.
    /// </summary>
    [PoolResettableMember]
    private long _lastCallTicks;
    /// <summary>
    /// Indicates whether <see cref="Initialize"/> has completed successfully. Enumerate calls return an empty sequence while this is false
    /// </summary>
    [PoolResettableMember]
    private bool _isInitialized;
    /// <summary>
    /// The sentinel that indicates no parameterless <see cref="Enumerate()"/> call has yet recorded a baseline tick.
    /// </summary>
    private const long UnsetLastCallTicks = 0;

    /// <summary>
    /// Creates an uninitialized cursor for use with the pool. <see cref="Initialize"/> must be called before any Enumerate call.
    /// </summary>
    public RoundRobinCursor() { }

    /// <summary>
    /// Creates a new cursor over the supplied collection.
    /// </summary>
    /// <param name="collection">The collection to walk. The reference is retained; subsequent additions and removals are observed on the next call to Enumerate.</param>
    /// <param name="sweepWindowMilliseconds">The total window, in milliseconds, over which one full pass of the collection should occur.</param>
    public RoundRobinCursor(IReadOnlyList<T0> collection, uint sweepWindowMilliseconds) => Initialize(collection, sweepWindowMilliseconds);

    /// <summary>
    /// Configures the cursor with a collection and sweep window. Logs an error and leaves the cursor unconfigured when the collection is null or the sweep window is zero.
    /// </summary>
    /// <param name="collection">The collection to walk. The reference is retained; subsequent additions and removals are observed on the next call to Enumerate.</param>
    /// <param name="sweepWindowMilliseconds">The total window, in milliseconds, over which one full pass of the collection should occur.</param>
    public void Initialize(IReadOnlyList<T0> collection, uint sweepWindowMilliseconds)
    {
        if (collection is null)
        {
            Logger<RoundRobinCursor<T0>>.LogError($"The collection cannot be null.");
            return;
        }

        if (!EnsureSweepWindow(sweepWindowMilliseconds))
            return;

        _collection = collection;
        _isInitialized = true;
    }

    /// <summary>
    /// Validates the supplied sweep window and stores it as the new value for subsequent Enumerate calls. Logs an error and leaves the existing value untouched when the supplied value is zero.
    /// </summary>
    /// <param name="sweepWindowMilliseconds">The new total window, in milliseconds, over which one full pass should occur. Must be greater than zero.</param>
    /// <returns>True when the value is valid and stored; false when the value is invalid and ignored.</returns>
    public bool EnsureSweepWindow(uint sweepWindowMilliseconds)
    {
        if (sweepWindowMilliseconds == 0)
        {
            Logger<RoundRobinCursor<T0>>.LogError($"The sweep window [{sweepWindowMilliseconds}] must be larger than 0.");

            return false;
        }

        _sweepWindowMilliseconds = sweepWindowMilliseconds;

        return true;
    }

    /// <summary>
    /// Yields the next wrap-around batch from the underlying collection, measuring elapsed time from <see cref="DateTime.UtcNow"/> ticks captured between calls.
    /// </summary>
    /// <returns>The next batch of elements. Empty when the collection is empty.</returns>
    public BatchEnumerable Enumerate()
    {
        if (!_isInitialized)
            return default;

        long currentTicks = DateTime.UtcNow.Ticks;
        long elapsedMilliseconds = 0;

        if (_lastCallTicks != UnsetLastCallTicks)
        {
            elapsedMilliseconds = (currentTicks - _lastCallTicks) / TimeSpan.TicksPerMillisecond;

            if (elapsedMilliseconds < 0)
                elapsedMilliseconds = 0;
            else if (elapsedMilliseconds > _sweepWindowMilliseconds)
                elapsedMilliseconds = _sweepWindowMilliseconds;
        }

        _lastCallTicks = currentTicks;

        return Enumerate((uint)elapsedMilliseconds);
    }

    /// <summary>
    /// Yields the next wrap-around batch from the underlying collection, sized according to the supplied elapsed time relative to the configured sweep window.
    /// </summary>
    /// <param name="elapsedMilliseconds">The number of milliseconds that have passed since the previous call. Used to size the returned batch as a fraction of the sweep window.</param>
    /// <returns>The next batch of elements. Empty when the collection is empty.</returns>
    public BatchEnumerable Enumerate(uint elapsedMilliseconds)
    {
        if (!_isInitialized)
            return default;

        int collectionCount = _collection.Count;
        if (collectionCount == 0)
            return default;

        long batchScaled = collectionCount * elapsedMilliseconds / _sweepWindowMilliseconds;
        int batchSize;

        if (batchScaled < 1)
            batchSize = 1;
        else if (batchScaled > collectionCount)
            batchSize = collectionCount;
        else
            batchSize = (int)batchScaled;

        return new(this, batchSize);
    }

    public void OnReturn()
    {
        _collection = null;
        _sweepWindowMilliseconds = 0;
        _lastCallTicks = UnsetLastCallTicks;
        _lastIndex = 0;
        _isInitialized = false;
    }

    public void OnRent() { }

    /// <summary>
    /// A non-allocating wrapper that exposes a <see cref="BatchEnumerator"/> for a sized batch produced by a <see cref="RoundRobinCursor{T0}"/>.
    /// </summary>
    public readonly struct BatchEnumerable : IEnumerable<T0>
    {
        /// <summary>
        /// The cursor whose state advances as the batch is consumed.
        /// </summary>
        private readonly RoundRobinCursor<T0> _cursor;
        /// <summary>
        /// The number of elements the batch is allowed to yield.
        /// </summary>
        private readonly int _batchSize;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal BatchEnumerable(RoundRobinCursor<T0> cursor, int batchSize)
        {
            _cursor = cursor;
            _batchSize = batchSize;
        }

        /// <summary>
        /// Returns a struct enumerator that walks the batch without allocations.
        /// </summary>
        /// <returns>A <see cref="BatchEnumerator"/> over the batch.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BatchEnumerator GetEnumerator() => new(_cursor, _batchSize);

        IEnumerator<T0> IEnumerable<T0>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// A struct enumerator that walks a single batch produced by a <see cref="RoundRobinCursor{T0}"/>, advancing the cursor's last-index as elements are consumed.
    /// </summary>
    public struct BatchEnumerator : IEnumerator<T0>
    {
        /// <summary>
        /// The element returned by the most recent successful <see cref="MoveNext"/> call.
        /// </summary>
        public T0 Current { get; private set; }

        /// <summary>
        /// The cursor whose state advances as the batch is consumed.
        /// </summary>
        private readonly RoundRobinCursor<T0> _cursor;
        /// <summary>
        /// The number of elements the batch is allowed to yield.
        /// </summary>
        private readonly int _batchSize;
        /// <summary>
        /// The number of elements yielded so far by this enumerator.
        /// </summary>
        private int _consumed;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal BatchEnumerator(RoundRobinCursor<T0> cursor, int batchSize)
        {
            _cursor = cursor;
            _batchSize = batchSize;
            _consumed = 0;
            Current = default;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the batch and updates the cursor's last-index.
        /// </summary>
        /// <returns>True when an element was produced; false when the batch is exhausted.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (_cursor is null)
                return false;

            if (_consumed >= _batchSize)
                return false;

            IReadOnlyList<T0> collection = _cursor._collection;
            if (collection is null)
                return false;

            int collectionCount = collection.Count;
            if (collectionCount == 0)
                return false;

            uint lastIndex = _cursor._lastIndex;
            if (lastIndex >= collectionCount)
                lastIndex = 0;

            Current = collection[(int)lastIndex];
            _cursor._lastIndex = lastIndex + 1;
            _consumed++;

            return true;
        }

        /// <summary>
        /// Resets the consumed count for this enumerator. Does not rewind the cursor itself.
        /// </summary>
        public void Reset()
        {
            _consumed = 0;
            Current = default;
        }

        object IEnumerator.Current => Current;

        public void Dispose() { }
    }
}
