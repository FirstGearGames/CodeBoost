using System;
using CodeBoost.Networking;
using CodeBoost.Performance;
using Xunit;

namespace CodeBoost.Tests;

/// <summary>
/// Coverage for <see cref="PacketReassembler"/>: payloads rebuilt from segments in any order, inconsistent segments refused, the limit on
/// payloads in progress, and expiry.
/// </summary>
public class PacketReassemblerTests
{
    /// <summary>
    /// The most bytes a segment may carry in these tests.
    /// </summary>
    private const uint MaximumSegmentLength = 8;

    /// <summary>
    /// Segments arriving out of order rebuild the payload exactly, once, and leave nothing in progress.
    /// </summary>
    [Fact]
    public void SegmentsInAnyOrder_RebuildThePayload()
    {
        PacketReassembler reassembler = RentReassembler(maximumConcurrentAssemblies: 4);

        try
        {
            byte[] payload = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20];

            Assert.False(reassembler.TryReassemble(7, 2, 3, new(payload, 16, 4), isReliable: false, nowTicks: 0, out _, out _, out _));
            Assert.False(reassembler.TryReassemble(7, 0, 3, new(payload, 0, 8), isReliable: false, nowTicks: 0, out _, out _, out _));
            Assert.True(reassembler.TryReassemble(7, 1, 3, new(payload, 8, 8), isReliable: false, nowTicks: 0, out ArraySegment<byte> assembled, out bool isProtocolViolation, out _));

            Assert.False(isProtocolViolation);
            Assert.Equal(payload, assembled.ToArray());
            Assert.False(reassembler.HasAssemblies);
            TrackedArrayPool<byte>.Return(assembled.Array!);
        }
        finally
        {
            ResettableObjectPool<PacketReassembler>.Return(reassembler);
        }
    }

    /// <summary>
    /// A segment already held is reported as a duplicate and does not complete the payload twice.
    /// </summary>
    [Fact]
    public void DuplicateSegment_IsReportedAndIgnored()
    {
        PacketReassembler reassembler = RentReassembler(maximumConcurrentAssemblies: 4);

        try
        {
            byte[] segment = [1, 2];

            reassembler.TryReassemble(3, 0, 2, new(segment), isReliable: true, nowTicks: 0, out _, out _, out bool isFirstDuplicate);
            reassembler.TryReassemble(3, 0, 2, new(segment), isReliable: true, nowTicks: 0, out _, out _, out bool isSecondDuplicate);

            Assert.False(isFirstDuplicate);
            Assert.True(isSecondDuplicate);
            Assert.True(reassembler.HasAssemblies);
        }
        finally
        {
            ResettableObjectPool<PacketReassembler>.Return(reassembler);
        }
    }

    /// <summary>
    /// A segment longer than the maximum, or one whose shape differs from its payload's, is a protocol violation, and the second discards
    /// the payload it names.
    /// </summary>
    [Fact]
    public void InconsistentSegments_AreProtocolViolations()
    {
        PacketReassembler reassembler = RentReassembler(maximumConcurrentAssemblies: 4);

        try
        {
            reassembler.TryReassemble(1, 0, 2, new(new byte[MaximumSegmentLength + 1]), isReliable: false, nowTicks: 0, out _, out bool isOversizedViolation, out _);
            Assert.True(isOversizedViolation, "a segment longer than the maximum was accepted");
            Assert.False(reassembler.HasAssemblies);

            reassembler.TryReassemble(2, 0, 2, new(new byte[4]), isReliable: false, nowTicks: 0, out _, out _, out _);
            reassembler.TryReassemble(2, 1, 3, new(new byte[4]), isReliable: false, nowTicks: 0, out _, out bool isMismatchViolation, out _);
            Assert.True(isMismatchViolation, "a segment declaring a different count was accepted");
            Assert.False(reassembler.HasAssemblies);
        }
        finally
        {
            ResettableObjectPool<PacketReassembler>.Return(reassembler);
        }
    }

    /// <summary>
    /// At the limit, a new payload evicts the oldest incomplete unreliable one, and when every payload in progress is reliable the new
    /// segment is dropped without a violation instead.
    /// </summary>
    [Fact]
    public void AtTheLimit_TheOldestUnreliableIsEvicted_AndReliableOnesAreKept()
    {
        PacketReassembler reassembler = RentReassembler(maximumConcurrentAssemblies: 2);
        byte[] segment = [1];
        ArraySegment<byte> bitmap = new(new byte[32]);

        try
        {
            reassembler.TryReassemble(1, 0, 2, new(segment), isReliable: false, nowTicks: 10, out _, out _, out _);
            reassembler.TryReassemble(2, 0, 2, new(segment), isReliable: false, nowTicks: 20, out _, out _, out _);
            reassembler.TryReassemble(3, 0, 2, new(segment), isReliable: false, nowTicks: 30, out _, out bool isUnreliableViolation, out _);

            Assert.False(isUnreliableViolation);
            Assert.False(reassembler.TryWriteReceivedBitmap(1, bitmap, out _), "the oldest unreliable payload was not evicted");
            Assert.True(reassembler.TryWriteReceivedBitmap(2, bitmap, out _) && reassembler.TryWriteReceivedBitmap(3, bitmap, out _));

            ResettableObjectPool<PacketReassembler>.Return(reassembler);
            reassembler = RentReassembler(maximumConcurrentAssemblies: 2);

            reassembler.TryReassemble(1, 0, 2, new(segment), isReliable: true, nowTicks: 10, out _, out _, out _);
            reassembler.TryReassemble(2, 0, 2, new(segment), isReliable: true, nowTicks: 20, out _, out _, out _);
            reassembler.TryReassemble(3, 0, 2, new(segment), isReliable: true, nowTicks: 30, out _, out bool isReliableViolation, out _);

            Assert.False(isReliableViolation);
            Assert.True(reassembler.TryWriteReceivedBitmap(1, bitmap, out _) && reassembler.TryWriteReceivedBitmap(2, bitmap, out _), "a reliable payload was evicted");
            Assert.False(reassembler.TryWriteReceivedBitmap(3, bitmap, out _), "a reliable segment beyond the limit was buffered");
        }
        finally
        {
            ResettableObjectPool<PacketReassembler>.Return(reassembler);
        }
    }

    /// <summary>
    /// A payload whose first segment arrived longer ago than the timeout is discarded, and a younger one is kept.
    /// </summary>
    [Fact]
    public void ExpiredPayloads_AreDiscarded()
    {
        PacketReassembler reassembler = RentReassembler(maximumConcurrentAssemblies: 4);
        byte[] segment = [1];
        ArraySegment<byte> bitmap = new(new byte[32]);

        try
        {
            reassembler.TryReassemble(1, 0, 2, new(segment), isReliable: false, nowTicks: 100, out _, out _, out _);
            reassembler.TryReassemble(2, 0, 2, new(segment), isReliable: false, nowTicks: 150, out _, out _, out _);

            reassembler.RemoveExpiredSegments(nowTicks: 200, timeoutTicks: 75);

            Assert.False(reassembler.TryWriteReceivedBitmap(1, bitmap, out _), "an expired payload was kept");
            Assert.True(reassembler.TryWriteReceivedBitmap(2, bitmap, out _), "a payload within the timeout was discarded");

            reassembler.RemoveExpiredSegments(nowTicks: 1000, timeoutTicks: 75);
            Assert.False(reassembler.HasAssemblies);
        }
        finally
        {
            ResettableObjectPool<PacketReassembler>.Return(reassembler);
        }
    }

    /// <summary>
    /// Rents and initializes a reassembler for these tests.
    /// </summary>
    /// <param name="maximumConcurrentAssemblies">The most payloads in progress at once.</param>
    /// <returns>The reassembler, to be returned to its pool.</returns>
    private static PacketReassembler RentReassembler(uint maximumConcurrentAssemblies)
    {
        PacketReassembler reassembler = ResettableObjectPool<PacketReassembler>.Rent();
        reassembler.Initialize(MaximumSegmentLength, PacketReassembler.MaximumSegmentCount, maximumConcurrentAssemblies);

        return reassembler;
    }
}
