using System.Collections.Generic;
using System.Linq;
using CodeBoost.Performance;
using CodeBoost.Types;
using Xunit;

namespace CodeBoost.Tests;

public class RingBufferTests
{
    /// <summary>
    /// Constructs a ring buffer and asserts the supplied capacity along with a clean initial state.
    /// </summary>
    private static RingBuffer<int> CreateBuffer(int capacity)
    {
        RingBuffer<int> ringBuffer = new(capacity);

        Assert.True(ringBuffer.Initialized);
        Assert.Equal(capacity, ringBuffer.Capacity);
        Assert.Equal(0, ringBuffer.Count);
        Assert.Equal(0, ringBuffer.WriteIndex);
        Assert.NotNull(ringBuffer.Collection);

        return ringBuffer;
    }

    // ─────────────────────────────────────────────────────────────────────
    // Construction
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_Default_UsesDefaultCapacity()
    {
        RingBuffer<int> ringBuffer = new();

        Assert.True(ringBuffer.Initialized);
        Assert.Equal(RingBuffer<int>.DefaultCapacity, ringBuffer.Capacity);
        Assert.Equal(0, ringBuffer.Count);
        Assert.Equal(0, ringBuffer.WriteIndex);
    }

    [Fact]
    public void Constructor_CustomCapacity_AppliesCapacity()
    {
        RingBuffer<int> ringBuffer = new(8);

        Assert.Equal(8, ringBuffer.Capacity);
    }

    [Fact]
    public void Constructor_CapacityOne_Works()
    {
        RingBuffer<int> ringBuffer = new(1);

        Assert.Equal(1, ringBuffer.Capacity);
        Assert.Equal(0, ringBuffer.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_NonPositiveCapacity_DoesNotInitialize(int capacity)
    {
        RingBuffer<int> ringBuffer = new(capacity);

        Assert.False(ringBuffer.Initialized);
    }

    // ─────────────────────────────────────────────────────────────────────
    // Add — under capacity
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void Add_SingleEntry_StoresAtIndexZero()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(5);

        int replaced = ringBuffer.Add(42);

        Assert.Equal(1, ringBuffer.Count);
        Assert.Equal(0, replaced);
        Assert.Equal(42, ringBuffer[0]);
        Assert.Equal(1, ringBuffer.WriteIndex);
    }

    [Fact]
    public void Add_PartialFill_PreservesInsertionOrder()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(5);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);

        Assert.Equal(3, ringBuffer.Count);
        Assert.Equal(1, ringBuffer[0]);
        Assert.Equal(2, ringBuffer[1]);
        Assert.Equal(3, ringBuffer[2]);
        Assert.Equal(3, ringBuffer.WriteIndex);
    }

    [Fact]
    public void Add_ExactlyFillsCapacity_AllEntriesPresent()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        for (int i = 0; i < 4; i++)
            ringBuffer.Add(i + 1);

        Assert.Equal(4, ringBuffer.Count);
        Assert.Equal(1, ringBuffer[0]);
        Assert.Equal(2, ringBuffer[1]);
        Assert.Equal(3, ringBuffer[2]);
        Assert.Equal(4, ringBuffer[3]);
        Assert.Equal(0, ringBuffer.WriteIndex);
    }

    // ─────────────────────────────────────────────────────────────────────
    // Add — overflow / wrap
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void Add_Overflow_WrapsAndDropsOldest()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(3);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);

        int replacedA = ringBuffer.Add(4);
        int replacedB = ringBuffer.Add(5);

        Assert.Equal(3, ringBuffer.Count);
        Assert.Equal(1, replacedA);
        Assert.Equal(2, replacedB);
        Assert.Equal(3, ringBuffer[0]);
        Assert.Equal(4, ringBuffer[1]);
        Assert.Equal(5, ringBuffer[2]);
    }

    [Fact]
    public void Add_MultipleFullCycles_OnlyKeepsLastCapacityEntries()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        for (int i = 0; i < 20; i++)
            ringBuffer.Add(i);

        Assert.Equal(4, ringBuffer.Count);
        Assert.Equal(16, ringBuffer[0]);
        Assert.Equal(17, ringBuffer[1]);
        Assert.Equal(18, ringBuffer[2]);
        Assert.Equal(19, ringBuffer[3]);
    }

    [Fact]
    public void Add_ReturnsDefault_BeforeBufferFills()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(3);

        Assert.Equal(0, ringBuffer.Add(10));
        Assert.Equal(0, ringBuffer.Add(20));
        Assert.Equal(0, ringBuffer.Add(30));

        Assert.Equal(10, ringBuffer.Add(40));
    }

    [Fact]
    public void Enqueue_BehavesLikeAdd()
    {
        RingBuffer<int> ringBufferA = CreateBuffer(3);
        RingBuffer<int> ringBufferB = CreateBuffer(3);

        for (int i = 0; i < 5; i++)
        {
            ringBufferA.Add(i);
            ringBufferB.Enqueue(i);
        }

        Assert.Equal(ringBufferA.Count, ringBufferB.Count);

        for (int i = 0; i < ringBufferA.Count; i++)
            Assert.Equal(ringBufferA[i], ringBufferB[i]);
    }

    // ─────────────────────────────────────────────────────────────────────
    // Indexer
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void Indexer_Set_UpdatesValue()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);

        ringBuffer[1] = 99;

        Assert.Equal(99, ringBuffer[1]);
        Assert.Equal(1, ringBuffer[0]);
        Assert.Equal(3, ringBuffer[2]);
    }

    [Fact]
    public void Indexer_Set_AfterWrap_TargetsCorrectSimulatedSlot()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(3);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);
        ringBuffer.Add(4);

        ringBuffer[0] = 100;

        Assert.Equal(100, ringBuffer[0]);
        Assert.Equal(3, ringBuffer[1]);
        Assert.Equal(4, ringBuffer[2]);
    }

    [Fact]
    public void Indexer_GetOutOfRange_ReturnsDefault()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Add(1);
        ringBuffer.Add(2);

        Assert.Equal(0, ringBuffer[5]);
        Assert.Equal(0, ringBuffer[2]);
        Assert.Equal(0, ringBuffer[-1]);
    }

    [Fact]
    public void Indexer_SetOutOfRange_DoesNotMutate()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Add(1);
        ringBuffer.Add(2);

        ringBuffer[5] = 99;
        ringBuffer[-1] = 88;

        Assert.Equal(1, ringBuffer[0]);
        Assert.Equal(2, ringBuffer[1]);
    }

    // ─────────────────────────────────────────────────────────────────────
    // Iteration
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void Foreach_PartialFill_YieldsInsertionOrder()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(5);

        ringBuffer.Add(10);
        ringBuffer.Add(20);
        ringBuffer.Add(30);

        List<int> collected = new();
        foreach (int value in ringBuffer)
            collected.Add(value);

        Assert.Equal(new[] { 10, 20, 30 }, collected);
    }

    [Fact]
    public void Foreach_AfterWrap_YieldsOldestToNewest()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(3);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);
        ringBuffer.Add(4);
        ringBuffer.Add(5);

        List<int> collected = new();
        foreach (int value in ringBuffer)
            collected.Add(value);

        Assert.Equal(new[] { 3, 4, 5 }, collected);
    }

    [Fact]
    public void Foreach_EmptyBuffer_YieldsNothing()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        List<int> collected = new();
        foreach (int value in ringBuffer)
            collected.Add(value);

        Assert.Empty(collected);
    }

    [Fact]
    public void Foreach_ModifiedDuringEnumeration_StopsEarly()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);

        RingBuffer<int>.Enumerator enumerator = ringBuffer.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.Equal(1, enumerator.Current);

        ringBuffer.Add(4);

        Assert.False(enumerator.MoveNext());
    }

    // ─────────────────────────────────────────────────────────────────────
    // Dequeue / TryDequeue
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void Dequeue_Empty_ReturnsDefault()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(3);

        Assert.Equal(0, ringBuffer.Dequeue());
        Assert.Equal(0, ringBuffer.Count);
    }

    [Fact]
    public void Dequeue_RemovesOldestAndDecrementsCount()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);

        Assert.Equal(1, ringBuffer.Dequeue());
        Assert.Equal(2, ringBuffer.Count);
        Assert.Equal(2, ringBuffer[0]);
        Assert.Equal(3, ringBuffer[1]);
    }

    [Fact]
    public void Dequeue_AfterWrap_ReturnsOldestSimulatedFirst()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(3);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);
        ringBuffer.Add(4);

        Assert.Equal(2, ringBuffer.Dequeue());
        Assert.Equal(3, ringBuffer.Dequeue());
        Assert.Equal(4, ringBuffer.Dequeue());
        Assert.Equal(0, ringBuffer.Count);
    }

    [Fact]
    public void TryDequeue_Empty_ReturnsFalseDefault()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(3);

        bool isDequeued = ringBuffer.TryDequeue(out int value);

        Assert.False(isDequeued);
        Assert.Equal(0, value);
    }

    [Fact]
    public void TryDequeue_NonEmpty_ReturnsTrueOldestValue()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(3);

        ringBuffer.Add(7);
        ringBuffer.Add(8);

        bool isDequeued = ringBuffer.TryDequeue(out int value);

        Assert.True(isDequeued);
        Assert.Equal(7, value);
        Assert.Equal(1, ringBuffer.Count);
        Assert.Equal(8, ringBuffer[0]);
    }

    // ─────────────────────────────────────────────────────────────────────
    // RemoveRange
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void RemoveRange_LengthZero_NoChange()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Add(1);
        ringBuffer.Add(2);

        ringBuffer.RemoveRange(fromStart: true, 0);

        Assert.Equal(2, ringBuffer.Count);
        Assert.Equal(1, ringBuffer[0]);
        Assert.Equal(2, ringBuffer[1]);
    }

    [Fact]
    public void RemoveRange_NegativeLength_NoChange()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Add(1);
        ringBuffer.Add(2);

        ringBuffer.RemoveRange(fromStart: true, -3);

        Assert.Equal(2, ringBuffer.Count);
    }

    [Fact]
    public void RemoveRange_FromStart_PartialKeepsTail()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(5);

        for (int i = 1; i <= 5; i++)
            ringBuffer.Add(i);

        ringBuffer.RemoveRange(fromStart: true, 2);

        Assert.Equal(3, ringBuffer.Count);
        Assert.Equal(3, ringBuffer[0]);
        Assert.Equal(4, ringBuffer[1]);
        Assert.Equal(5, ringBuffer[2]);
    }

    [Fact]
    public void RemoveRange_FromEnd_PartialKeepsHead()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(5);

        for (int i = 1; i <= 5; i++)
            ringBuffer.Add(i);

        ringBuffer.RemoveRange(fromStart: false, 2);

        Assert.Equal(3, ringBuffer.Count);
        Assert.Equal(1, ringBuffer[0]);
        Assert.Equal(2, ringBuffer[1]);
        Assert.Equal(3, ringBuffer[2]);
    }

    [Fact]
    public void RemoveRange_LengthEqualsWritten_ClearsBuffer()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(5);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);

        ringBuffer.RemoveRange(fromStart: true, 3);

        Assert.Equal(0, ringBuffer.Count);
        Assert.Equal(0, ringBuffer.WriteIndex);
    }

    [Fact]
    public void RemoveRange_LengthExceedsWritten_ClearsBuffer()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(5);

        ringBuffer.Add(1);
        ringBuffer.Add(2);

        ringBuffer.RemoveRange(fromStart: false, 99);

        Assert.Equal(0, ringBuffer.Count);
    }

    [Fact]
    public void RemoveRange_FromStart_AfterWrap_KeepsCorrectTail()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);
        ringBuffer.Add(4);
        ringBuffer.Add(5);
        ringBuffer.Add(6);

        ringBuffer.RemoveRange(fromStart: true, 2);

        Assert.Equal(2, ringBuffer.Count);
        Assert.Equal(5, ringBuffer[0]);
        Assert.Equal(6, ringBuffer[1]);
    }

    [Fact]
    public void RemoveRange_FromEnd_AfterWrap_KeepsCorrectHead()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);
        ringBuffer.Add(4);
        ringBuffer.Add(5);
        ringBuffer.Add(6);

        ringBuffer.RemoveRange(fromStart: false, 2);

        Assert.Equal(2, ringBuffer.Count);
        Assert.Equal(3, ringBuffer[0]);
        Assert.Equal(4, ringBuffer[1]);
    }

    // ─────────────────────────────────────────────────────────────────────
    // Insert
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void Insert_AtZero_EmptyBuffer_AddsEntry()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Insert(0, 42);

        Assert.Equal(1, ringBuffer.Count);
        Assert.Equal(42, ringBuffer[0]);
    }

    [Fact]
    public void Insert_AtZero_PartialBuffer_ShiftsRight()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(5);

        ringBuffer.Add(2);
        ringBuffer.Add(3);
        ringBuffer.Add(4);

        ringBuffer.Insert(0, 1);

        Assert.Equal(4, ringBuffer.Count);
        Assert.Equal(1, ringBuffer[0]);
        Assert.Equal(2, ringBuffer[1]);
        Assert.Equal(3, ringBuffer[2]);
        Assert.Equal(4, ringBuffer[3]);
    }

    [Fact]
    public void Insert_AtMiddle_PartialBuffer_ShiftsRight()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(6);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(4);
        ringBuffer.Add(5);

        ringBuffer.Insert(2, 3);

        Assert.Equal(5, ringBuffer.Count);
        Assert.Equal(1, ringBuffer[0]);
        Assert.Equal(2, ringBuffer[1]);
        Assert.Equal(3, ringBuffer[2]);
        Assert.Equal(4, ringBuffer[3]);
        Assert.Equal(5, ringBuffer[4]);
    }

    [Fact]
    public void Insert_AtLastSimulatedIndex_ShiftsCorrectly()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(5);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(4);

        ringBuffer.Insert(2, 3);

        Assert.Equal(4, ringBuffer.Count);
        Assert.Equal(1, ringBuffer[0]);
        Assert.Equal(2, ringBuffer[1]);
        Assert.Equal(3, ringBuffer[2]);
        Assert.Equal(4, ringBuffer[3]);
    }

    [Fact]
    public void Insert_AtWrittenIndex_AppendsAtEnd()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(5);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);

        ringBuffer.Insert(3, 4);

        Assert.Equal(4, ringBuffer.Count);
        Assert.Equal(1, ringBuffer[0]);
        Assert.Equal(2, ringBuffer[1]);
        Assert.Equal(3, ringBuffer[2]);
        Assert.Equal(4, ringBuffer[3]);
    }

    [Fact]
    public void Insert_OutOfRange_ReturnsDefaultAndDoesNotMutate()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Add(1);
        ringBuffer.Add(2);

        int previous = ringBuffer.Insert(7, 99);

        Assert.Equal(0, previous);
        Assert.Equal(2, ringBuffer.Count);
        Assert.Equal(1, ringBuffer[0]);
        Assert.Equal(2, ringBuffer[1]);
    }

    [Fact]
    public void Insert_AtZero_AfterWrap_ShiftsRightWithinSimulatedWindow()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);
        ringBuffer.Add(4);
        ringBuffer.Add(5);
        ringBuffer.Add(6);
        ringBuffer.Dequeue();

        ringBuffer.Insert(0, 10);

        Assert.Equal(10, ringBuffer[0]);
        Assert.Equal(4, ringBuffer[1]);
        Assert.Equal(5, ringBuffer[2]);
        Assert.Equal(6, ringBuffer[3]);
    }

    // ─────────────────────────────────────────────────────────────────────
    // Clear
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void Clear_EmptyBuffer_RemainsClean()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Clear();

        Assert.Equal(0, ringBuffer.Count);
        Assert.Equal(0, ringBuffer.WriteIndex);
    }

    [Fact]
    public void Clear_PartiallyFilled_ResetsState()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);

        ringBuffer.Clear();

        Assert.Equal(0, ringBuffer.Count);
        Assert.Equal(0, ringBuffer.WriteIndex);

        for (int i = 0; i < ringBuffer.Capacity; i++)
            Assert.Equal(0, ringBuffer.Collection[i]);
    }

    [Fact]
    public void Clear_FullBuffer_ResetsState()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(3);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Add(3);
        ringBuffer.Add(4);

        ringBuffer.Clear();

        Assert.Equal(0, ringBuffer.Count);
        Assert.Equal(0, ringBuffer.WriteIndex);

        for (int i = 0; i < ringBuffer.Capacity; i++)
            Assert.Equal(0, ringBuffer.Collection[i]);
    }

    [Fact]
    public void Clear_FollowedByAdd_BehavesAsFreshBuffer()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(3);

        ringBuffer.Add(1);
        ringBuffer.Add(2);
        ringBuffer.Clear();

        Assert.Equal(0, ringBuffer.Add(99));
        Assert.Equal(99, ringBuffer[0]);
        Assert.Equal(1, ringBuffer.Count);
    }

    // ─────────────────────────────────────────────────────────────────────
    // GC retention for reference T0
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void Dequeue_ReferenceType_NullsVacatedSlot()
    {
        RingBuffer<string> ringBuffer = new(4);
        ringBuffer.Add("a");
        ringBuffer.Add("b");
        ringBuffer.Add("c");

        int oldStart = ringBuffer.Capacity - ringBuffer.Count + ringBuffer.WriteIndex;
        if (oldStart >= ringBuffer.Capacity)
            oldStart -= ringBuffer.Capacity;

        ringBuffer.Dequeue();

        Assert.Null(ringBuffer.Collection[oldStart]);
        Assert.Equal("b", ringBuffer[0]);
        Assert.Equal("c", ringBuffer[1]);
    }

    [Fact]
    public void RemoveRangeFromEnd_ReferenceType_NullsVacatedSlots()
    {
        RingBuffer<string> ringBuffer = new(4);
        ringBuffer.Add("a");
        ringBuffer.Add("b");
        ringBuffer.Add("c");
        ringBuffer.Add("d");

        ringBuffer.RemoveRange(fromStart: false, 2);

        Assert.Equal(2, ringBuffer.Count);
        Assert.Equal("a", ringBuffer[0]);
        Assert.Equal("b", ringBuffer[1]);
        Assert.Null(ringBuffer.Collection[2]);
        Assert.Null(ringBuffer.Collection[3]);
    }

    [Fact]
    public void Clear_ReferenceType_NullsAllSlots()
    {
        RingBuffer<string> ringBuffer = new(3);
        ringBuffer.Add("a");
        ringBuffer.Add("b");
        ringBuffer.Add("c");
        ringBuffer.Add("d");

        ringBuffer.Clear();

        for (int i = 0; i < ringBuffer.Capacity; i++)
            Assert.Null(ringBuffer.Collection[i]);
    }

    // ─────────────────────────────────────────────────────────────────────
    // Re-initialization
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void Initialize_SameCapacity_ResetsContent()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(4);

        ringBuffer.Add(1);
        ringBuffer.Add(2);

        ringBuffer.Initialize(4);

        Assert.Equal(4, ringBuffer.Capacity);
        Assert.Equal(0, ringBuffer.Count);
        Assert.Equal(0, ringBuffer.WriteIndex);
    }

    [Fact]
    public void Initialize_LargerCapacity_GrowsArray()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(2);

        ringBuffer.Add(1);
        ringBuffer.Add(2);

        ringBuffer.Initialize(64);

        Assert.Equal(64, ringBuffer.Capacity);
        Assert.True(ringBuffer.Collection.Length >= 64);
        Assert.Equal(0, ringBuffer.Count);
    }

    [Fact]
    public void Initialize_SmallerCapacity_ReducesCapacity()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(16);

        for (int i = 0; i < 10; i++)
            ringBuffer.Add(i);

        ringBuffer.Initialize(4);

        Assert.Equal(4, ringBuffer.Capacity);
        Assert.Equal(0, ringBuffer.Count);
    }

    // ─────────────────────────────────────────────────────────────────────
    // Capacity 1 edge
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void Capacity1_AddOverwritesPreviousValue()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(1);

        Assert.Equal(0, ringBuffer.Add(7));
        Assert.Equal(1, ringBuffer.Count);
        Assert.Equal(7, ringBuffer[0]);

        Assert.Equal(7, ringBuffer.Add(9));
        Assert.Equal(1, ringBuffer.Count);
        Assert.Equal(9, ringBuffer[0]);
    }

    [Fact]
    public void Capacity1_DequeueEmptiesBuffer()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(1);

        ringBuffer.Add(7);

        Assert.Equal(7, ringBuffer.Dequeue());
        Assert.Equal(0, ringBuffer.Count);
        Assert.Equal(0, ringBuffer.Add(11));
        Assert.Equal(11, ringBuffer[0]);
    }

    // ─────────────────────────────────────────────────────────────────────
    // Pool integration
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void Pool_RentReturnRent_ReusesAndClearsInstance()
    {
        RingBuffer<int> firstRented = RingBufferPool<int>.Rent();

        firstRented.Add(1);
        firstRented.Add(2);

        RingBufferPool<int>.Return(firstRented);

        RingBuffer<int> secondRented = RingBufferPool<int>.Rent();

        Assert.Equal(0, secondRented.Count);
        Assert.Equal(0, secondRented.WriteIndex);

        RingBufferPool<int>.Return(secondRented);
    }

    [Fact]
    public void Pool_ReturnAndNullifyReference_NullsCallerVariable()
    {
        RingBuffer<int>? rented = RingBufferPool<int>.Rent();
        rented.Add(5);

        RingBufferPool<int>.ReturnAndNullifyReference(ref rented!);

        Assert.Null(rented);
    }

    [Fact]
    public void Pool_ReturnNull_NoOp()
    {
        RingBuffer<int>? buffer = null;

        RingBufferPool<int>.Return(buffer!);
    }

    // ─────────────────────────────────────────────────────────────────────
    // Linq integration as IEnumerable<T0>
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void IEnumerable_Linq_ProducesExpectedSequence()
    {
        RingBuffer<int> ringBuffer = CreateBuffer(5);

        for (int i = 1; i <= 7; i++)
            ringBuffer.Add(i);

        IEnumerable<int> source = ringBuffer;

        Assert.Equal(new[] { 3, 4, 5, 6, 7 }, source.ToArray());
    }
}
