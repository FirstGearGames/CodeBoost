using CodeBoost.Performance;
using CodeBoost.Types;
using Xunit;

namespace CodeBoost.Tests;

public class BoostedQueueTests
{
    [Fact]
    public void Empty_ZeroCount()
    {
        BoostedQueue<int> queue = new();

        Assert.Equal(0, queue.Count);
        Assert.True(queue.Capacity > 0);
    }

    [Fact]
    public void Enqueue_SingleEntry_StoresAndIncrementsCount()
    {
        BoostedQueue<int> queue = new();

        queue.Enqueue(42);

        Assert.Equal(1, queue.Count);
        Assert.Equal(42, queue[0]);
    }

    [Fact]
    public void Enqueue_MultipleEntries_PreservesInsertionOrder()
    {
        BoostedQueue<int> queue = new();

        for (int i = 0; i < 5; i++)
            queue.Enqueue(i);

        Assert.Equal(5, queue.Count);

        for (int i = 0; i < 5; i++)
            Assert.Equal(i, queue[i]);
    }

    [Fact]
    public void Enqueue_BeyondInitialCapacity_TriggersResize()
    {
        BoostedQueue<int> queue = new();
        int initialCapacity = queue.Capacity;

        for (int i = 0; i < initialCapacity * 4; i++)
            queue.Enqueue(i);

        Assert.True(queue.Capacity > initialCapacity);
        Assert.Equal(initialCapacity * 4, queue.Count);

        for (int i = 0; i < queue.Count; i++)
            Assert.Equal(i, queue[i]);
    }

    [Fact]
    public void Dequeue_Empty_ReturnsDefault()
    {
        BoostedQueue<int> queue = new();

        Assert.Equal(0, queue.Dequeue());
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void Dequeue_RemovesOldestAndDecrementsCount()
    {
        BoostedQueue<int> queue = new();

        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);

        Assert.Equal(1, queue.Dequeue());
        Assert.Equal(2, queue.Count);
        Assert.Equal(2, queue[0]);
        Assert.Equal(3, queue[1]);
    }

    [Fact]
    public void Dequeue_DefaultsArrayEntry_ByDefault()
    {
        BoostedQueue<string> queue = new();

        queue.Enqueue("a");
        queue.Enqueue("b");

        queue.Dequeue();

        Assert.Equal(1, queue.Count);
        Assert.Equal("b", queue[0]);
    }

    [Fact]
    public void Dequeue_AfterResize_PreservesQueueOrder()
    {
        BoostedQueue<int> queue = new();
        int initialCapacity = queue.Capacity;

        for (int i = 0; i < initialCapacity; i++)
            queue.Enqueue(i);

        queue.Dequeue();
        queue.Dequeue();

        for (int i = 100; i < 100 + initialCapacity; i++)
            queue.Enqueue(i);

        Assert.Equal(2, queue[0]);
        Assert.Equal(initialCapacity - 1, queue[initialCapacity - 3]);
        Assert.Equal(100, queue[initialCapacity - 2]);
    }

    [Fact]
    public void TryDequeue_Empty_ReturnsFalse()
    {
        BoostedQueue<int> queue = new();

        bool isDequeued = queue.TryDequeue(out int value);

        Assert.False(isDequeued);
        Assert.Equal(0, value);
    }

    [Fact]
    public void TryDequeue_NonEmpty_ReturnsTrueValue()
    {
        BoostedQueue<int> queue = new();

        queue.Enqueue(7);

        bool isDequeued = queue.TryDequeue(out int value);

        Assert.True(isDequeued);
        Assert.Equal(7, value);
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void Peek_Empty_Throws()
    {
        BoostedQueue<int> queue = new();

        Assert.ThrowsAny<System.Exception>(() => queue.Peek());
    }

    [Fact]
    public void Peek_DoesNotMutateQueue()
    {
        BoostedQueue<int> queue = new();

        queue.Enqueue(5);
        queue.Enqueue(6);

        Assert.Equal(5, queue.Peek());
        Assert.Equal(2, queue.Count);
        Assert.Equal(5, queue[0]);
    }

    [Fact]
    public void TryPeek_Empty_ReturnsFalse()
    {
        BoostedQueue<int> queue = new();

        bool isPeeked = queue.TryPeek(out int value);

        Assert.False(isPeeked);
        Assert.Equal(0, value);
    }

    [Fact]
    public void TryPeek_NonEmpty_ReturnsTrueOldestValue()
    {
        BoostedQueue<int> queue = new();

        queue.Enqueue(99);

        bool isPeeked = queue.TryPeek(out int value);

        Assert.True(isPeeked);
        Assert.Equal(99, value);
        Assert.Equal(1, queue.Count);
    }

    [Fact]
    public void Indexer_OutOfRange_ThrowsAfterLogging()
    {
        BoostedQueue<int> queue = new();

        queue.Enqueue(1);

        Assert.ThrowsAny<System.Exception>(() => queue[5]);
    }

    [Fact]
    public void Indexer_Set_UpdatesValue()
    {
        BoostedQueue<int> queue = new();

        queue.Enqueue(1);
        queue.Enqueue(2);

        queue[0] = 99;

        Assert.Equal(99, queue[0]);
        Assert.Equal(2, queue[1]);
    }

    [Fact]
    public void GetIndexOrDefault_OutOfRange_ReturnsDefault()
    {
        BoostedQueue<int> queue = new();

        queue.Enqueue(1);

        Assert.Equal(0, queue.GetIndexOrDefault(5));
        Assert.Equal(0, queue.GetIndexOrDefault(-1));
    }

    [Fact]
    public void GetIndexOrDefault_InRange_ReturnsValue()
    {
        BoostedQueue<int> queue = new();

        queue.Enqueue(11);
        queue.Enqueue(22);

        Assert.Equal(11, queue.GetIndexOrDefault(0));
        Assert.Equal(22, queue.GetIndexOrDefault(1));
    }

    [Fact]
    public void Clear_ResetsCountAndIndexes()
    {
        BoostedQueue<int> queue = new();

        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);

        queue.Clear();

        Assert.Equal(0, queue.Count);
        Assert.Equal(0, queue.WriteIndex);
        Assert.Equal(0, queue.Dequeue());
    }

    [Fact]
    public void Clear_AllowsSubsequentEnqueue()
    {
        BoostedQueue<int> queue = new();

        queue.Enqueue(1);
        queue.Clear();

        queue.Enqueue(99);

        Assert.Equal(1, queue.Count);
        Assert.Equal(99, queue[0]);
    }

    [Fact]
    public void Pool_RentReturnRent_ReusesClearedInstance()
    {
        BoostedQueue<int> first = BoostedQueuePool<int>.Rent();
        first.Enqueue(1);
        first.Enqueue(2);

        BoostedQueuePool<int>.Return(first);

        BoostedQueue<int> second = BoostedQueuePool<int>.Rent();

        Assert.Equal(0, second.Count);

        BoostedQueuePool<int>.Return(second);
    }

    [Fact]
    public void Pool_ReturnAndNullifyReference_NullsCallerVariable()
    {
        BoostedQueue<int>? rented = BoostedQueuePool<int>.Rent();
        rented.Enqueue(5);

        BoostedQueuePool<int>.ReturnAndNullifyReference(ref rented!);

        Assert.Null(rented);
    }
}
