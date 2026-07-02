using CodeBoost.Performance;
using CodeBoost.Types;
using Xunit;

namespace CodeBoost.Tests;

public class ResettableFoldTests
{
    private sealed class CountingResettable : IPoolResettable
    {
        public int OnReturnCalls;
        public int OnRentCalls;

        public void OnReturn() => OnReturnCalls++;
        public void OnRent() => OnRentCalls++;
    }

    // ResettableRingBufferPool ──────────────────────────────────────────

    [Fact]
    public void ResettableRingBufferPool_Reset_CallsOnReturnOncePerPopulatedItem()
    {
        RingBuffer<CountingResettable> buffer = new(8);

        CountingResettable a = new();
        CountingResettable b = new();
        CountingResettable c = new();

        buffer.Add(a);
        buffer.Add(b);
        buffer.Add(c);

        ResettableRingBufferPool<CountingResettable>.Reset(buffer);

        Assert.Equal(1, a.OnReturnCalls);
        Assert.Equal(1, b.OnReturnCalls);
        Assert.Equal(1, c.OnReturnCalls);
    }

    [Fact]
    public void ResettableRingBufferPool_Reset_NullsRefSlots()
    {
        RingBuffer<CountingResettable> buffer = new(4);

        buffer.Add(new());
        buffer.Add(new());
        buffer.Add(new());

        ResettableRingBufferPool<CountingResettable>.Reset(buffer);

        for (int i = 0; i < buffer.Capacity; i++)
            Assert.Null(buffer.Collection[i]);
    }

    [Fact]
    public void ResettableRingBufferPool_Reset_ResetsWriteState()
    {
        RingBuffer<CountingResettable> buffer = new(8);

        buffer.Add(new());
        buffer.Add(new());

        ResettableRingBufferPool<CountingResettable>.Reset(buffer);

        Assert.Equal(0, buffer.Count);
        Assert.Equal(0, buffer.WriteIndex);
    }

    [Fact]
    public void ResettableRingBufferPool_Reset_AfterWrap_HandlesAllPopulatedSlots()
    {
        RingBuffer<CountingResettable> buffer = new(3);

        CountingResettable[] items = new CountingResettable[5];
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = new();
            buffer.Add(items[i]);
        }

        ResettableRingBufferPool<CountingResettable>.Reset(buffer);

        // Items 0,1 were overwritten before Reset so OnReturn should not have been called.
        Assert.Equal(0, items[0].OnReturnCalls);
        Assert.Equal(0, items[1].OnReturnCalls);
        // Items 2,3,4 occupied the buffer at Reset so OnReturn should have been called once each.
        Assert.Equal(1, items[2].OnReturnCalls);
        Assert.Equal(1, items[3].OnReturnCalls);
        Assert.Equal(1, items[4].OnReturnCalls);
    }

    [Fact]
    public void ResettableRingBufferPool_Reset_DoesNotReturnToPool()
    {
        RingBuffer<CountingResettable> first = ResettableRingBufferPool<CountingResettable>.Rent();
        first.Add(new());

        ResettableRingBufferPool<CountingResettable>.Reset(first);

        RingBuffer<CountingResettable> second = ResettableRingBufferPool<CountingResettable>.Rent();

        Assert.NotSame(first, second);

        ResettableRingBufferPool<CountingResettable>.Return(second);
    }

    [Fact]
    public void ResettableRingBufferPool_Return_PushesToPool()
    {
        RingBuffer<CountingResettable> first = ResettableRingBufferPool<CountingResettable>.Rent();
        first.Add(new());
        first.Add(new());

        ResettableRingBufferPool<CountingResettable>.Return(first);

        RingBuffer<CountingResettable> second = ResettableRingBufferPool<CountingResettable>.Rent();

        Assert.Same(first, second);
        Assert.Equal(0, second.Count);

        ResettableRingBufferPool<CountingResettable>.Return(second);
    }

    [Fact]
    public void ResettableRingBufferPool_ReturnAndNullifyReference_NullsCallerVariable()
    {
        RingBuffer<CountingResettable>? rented = ResettableRingBufferPool<CountingResettable>.Rent();
        rented.Add(new());

        ResettableRingBufferPool<CountingResettable>.ReturnAndNullifyReference(ref rented!);

        Assert.Null(rented);
    }

    [Fact]
    public void ResettableRingBufferPool_Return_NullValue_NoOp()
    {
        RingBuffer<CountingResettable>? value = null;

        ResettableRingBufferPool<CountingResettable>.Return(value!);
    }

    // ResettableBoostedQueuePool ────────────────────────────────────────

    [Fact]
    public void ResettableBoostedQueuePool_Reset_CallsOnReturnOncePerItem()
    {
        BoostedQueue<CountingResettable> queue = new();

        CountingResettable a = new();
        CountingResettable b = new();
        CountingResettable c = new();

        queue.Enqueue(a);
        queue.Enqueue(b);
        queue.Enqueue(c);

        ResettableBoostedQueuePool<CountingResettable>.Reset(queue);

        Assert.Equal(1, a.OnReturnCalls);
        Assert.Equal(1, b.OnReturnCalls);
        Assert.Equal(1, c.OnReturnCalls);
    }

    [Fact]
    public void ResettableBoostedQueuePool_Reset_DrainsAndResetsState()
    {
        BoostedQueue<CountingResettable> queue = new();

        queue.Enqueue(new());
        queue.Enqueue(new());

        ResettableBoostedQueuePool<CountingResettable>.Reset(queue);

        Assert.Equal(0, queue.Count);
        Assert.Equal(0, queue.WriteIndex);
    }

    [Fact]
    public void ResettableBoostedQueuePool_Return_PushesToPool()
    {
        BoostedQueue<CountingResettable> first = ResettableBoostedQueuePool<CountingResettable>.Rent();
        first.Enqueue(new());

        ResettableBoostedQueuePool<CountingResettable>.Return(first);

        BoostedQueue<CountingResettable> second = ResettableBoostedQueuePool<CountingResettable>.Rent();

        Assert.Same(first, second);
        Assert.Equal(0, second.Count);

        ResettableBoostedQueuePool<CountingResettable>.Return(second);
    }

    [Fact]
    public void ResettableBoostedQueuePool_Return_AfterReturnAndRent_QueueIsClean()
    {
        BoostedQueue<CountingResettable> first = ResettableBoostedQueuePool<CountingResettable>.Rent();
        first.Enqueue(new());
        first.Enqueue(new());
        first.Enqueue(new());

        ResettableBoostedQueuePool<CountingResettable>.Return(first);

        BoostedQueue<CountingResettable> second = ResettableBoostedQueuePool<CountingResettable>.Rent();

        Assert.Equal(0, second.Count);
        Assert.False(second.TryPeek(out CountingResettable? _));

        ResettableBoostedQueuePool<CountingResettable>.Return(second);
    }

    [Fact]
    public void ResettableBoostedQueuePool_ReturnAndNullifyReference_NullsCallerVariable()
    {
        BoostedQueue<CountingResettable>? rented = ResettableBoostedQueuePool<CountingResettable>.Rent();
        rented.Enqueue(new());

        ResettableBoostedQueuePool<CountingResettable>.ReturnAndNullifyReference(ref rented!);

        Assert.Null(rented);
    }

    [Fact]
    public void ResettableBoostedQueuePool_Return_NullValue_NoOp()
    {
        BoostedQueue<CountingResettable>? value = null;

        ResettableBoostedQueuePool<CountingResettable>.Return(value!);
    }
}
