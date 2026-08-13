using System;
using System.Collections.Generic;
using CodeBoost.Performance;
using CodeBoost.Types;
using Xunit;

namespace CodeBoost.Tests;

/// <summary>
/// Proves every Resettable* collection pool completes its entries' life cycle: returning the collection returns each
/// contained <see cref="IPoolResettable"/> instance to its own <see cref="ResettableObjectPool{T0}"/> — with
/// <see cref="IPoolResettable.OnReturn"/> run exactly once — rather than resetting the instance and dropping it as garbage.
/// Every test rents its entries back within the same method, so the thread-local pool stacks make the identity checks
/// deterministic, and each test uses its own entry type so the type-keyed static pools cannot bleed between tests.
/// </summary>
public class ResettablePoolEntryReturnTests
{
    /// <summary>
    /// The value held in a returned SortedList re-enters its object pool: the next rent hands back the same instance,
    /// with OnReturn having run exactly once. This is the regression the receive path leaked through — reset-then-drop
    /// left the pool permanently empty, so every rent allocated.
    /// </summary>
    [Fact]
    public void T1SortedListPool_Return_ReturnsValuesToTheirPool()
    {
        SortedListValue value = ResettableObjectPool<SortedListValue>.Rent();
        SortedList<uint, SortedListValue> list = ResettableT1SortedListPool<uint, SortedListValue>.Rent();
        list.Add(1, value);

        ResettableT1SortedListPool<uint, SortedListValue>.Return(list);

        Assert.Equal(1, value.OnReturnCalls);
        Assert.Same(value, ResettableObjectPool<SortedListValue>.Rent());
    }

    /// <summary>
    /// Returning a key-resettable SortedList resets its keys and re-pools them. Return previously skipped Reset entirely,
    /// so keys neither ran OnReturn nor re-entered their pool.
    /// </summary>
    [Fact]
    public void T0SortedListPool_Return_ResetsAndReturnsKeysToTheirPool()
    {
        SortedListKey key = ResettableObjectPool<SortedListKey>.Rent();
        key.Ordinal = 1;

        SortedList<SortedListKey, uint> list = ResettableT0SortedListPool<SortedListKey, uint>.Rent();
        list.Add(key, 10);

        ResettableT0SortedListPool<SortedListKey, uint>.Return(list);

        Assert.Equal(1, key.OnReturnCalls);
        Assert.Same(key, ResettableObjectPool<SortedListKey>.Rent());
    }

    /// <summary>
    /// Returning a both-resettable SortedList re-pools keys and values alike. Reset previously touched only the keys,
    /// leaving every value un-reset and unpooled.
    /// </summary>
    [Fact]
    public void T0T1SortedListPool_Return_ReturnsKeysAndValuesToTheirPools()
    {
        PairedSortedListKey key = ResettableObjectPool<PairedSortedListKey>.Rent();
        key.Ordinal = 1;
        PairedSortedListValue value = ResettableObjectPool<PairedSortedListValue>.Rent();

        SortedList<PairedSortedListKey, PairedSortedListValue> list = ResettableT0T1SortedListPool<PairedSortedListKey, PairedSortedListValue>.Rent();
        list.Add(key, value);

        ResettableT0T1SortedListPool<PairedSortedListKey, PairedSortedListValue>.Return(list);

        Assert.Equal(1, key.OnReturnCalls);
        Assert.Equal(1, value.OnReturnCalls);
        Assert.Same(key, ResettableObjectPool<PairedSortedListKey>.Rent());
        Assert.Same(value, ResettableObjectPool<PairedSortedListValue>.Rent());
    }

    /// <summary>
    /// The key held in a returned Dictionary re-enters its object pool.
    /// </summary>
    [Fact]
    public void T0DictionaryPool_Return_ReturnsKeysToTheirPool()
    {
        DictionaryKey key = ResettableObjectPool<DictionaryKey>.Rent();
        Dictionary<DictionaryKey, uint> dictionary = ResettableT0DictionaryPool<DictionaryKey, uint>.Rent();
        dictionary.Add(key, 10);

        ResettableT0DictionaryPool<DictionaryKey, uint>.Return(dictionary);

        Assert.Equal(1, key.OnReturnCalls);
        Assert.Same(key, ResettableObjectPool<DictionaryKey>.Rent());
    }

    /// <summary>
    /// The value held in a returned Dictionary re-enters its object pool.
    /// </summary>
    [Fact]
    public void T1DictionaryPool_Return_ReturnsValuesToTheirPool()
    {
        DictionaryValue value = ResettableObjectPool<DictionaryValue>.Rent();
        Dictionary<uint, DictionaryValue> dictionary = ResettableT1DictionaryPool<uint, DictionaryValue>.Rent();
        dictionary.Add(1, value);

        ResettableT1DictionaryPool<uint, DictionaryValue>.Return(dictionary);

        Assert.Equal(1, value.OnReturnCalls);
        Assert.Same(value, ResettableObjectPool<DictionaryValue>.Rent());
    }

    /// <summary>
    /// Both the key and the value held in a returned Dictionary re-enter their object pools.
    /// </summary>
    [Fact]
    public void T0T1DictionaryPool_Return_ReturnsKeysAndValuesToTheirPools()
    {
        PairedDictionaryKey key = ResettableObjectPool<PairedDictionaryKey>.Rent();
        PairedDictionaryValue value = ResettableObjectPool<PairedDictionaryValue>.Rent();

        Dictionary<PairedDictionaryKey, PairedDictionaryValue> dictionary = ResettableT0T1DictionaryPool<PairedDictionaryKey, PairedDictionaryValue>.Rent();
        dictionary.Add(key, value);

        ResettableT0T1DictionaryPool<PairedDictionaryKey, PairedDictionaryValue>.Return(dictionary);

        Assert.Equal(1, key.OnReturnCalls);
        Assert.Equal(1, value.OnReturnCalls);
        Assert.Same(key, ResettableObjectPool<PairedDictionaryKey>.Rent());
        Assert.Same(value, ResettableObjectPool<PairedDictionaryValue>.Rent());
    }

    /// <summary>
    /// Entries drained from a reset BoostedQueue re-enter their object pool.
    /// </summary>
    [Fact]
    public void BoostedQueuePool_Reset_ReturnsEntriesToTheirPool()
    {
        QueueEntry entry = ResettableObjectPool<QueueEntry>.Rent();
        BoostedQueue<QueueEntry> queue = ResettableBoostedQueuePool<QueueEntry>.Rent();
        queue.Enqueue(entry);

        ResettableBoostedQueuePool<QueueEntry>.Return(queue);

        Assert.Equal(1, entry.OnReturnCalls);
        Assert.Same(entry, ResettableObjectPool<QueueEntry>.Rent());
    }

    /// <summary>
    /// Entries cleared from a reset RingBuffer re-enter their object pool.
    /// </summary>
    [Fact]
    public void RingBufferPool_Reset_ReturnsEntriesToTheirPool()
    {
        RingEntry entry = ResettableObjectPool<RingEntry>.Rent();
        RingBuffer<RingEntry> buffer = ResettableRingBufferPool<RingEntry>.Rent();
        buffer.Add(entry);

        ResettableRingBufferPool<RingEntry>.Return(buffer);

        Assert.Equal(1, entry.OnReturnCalls);
        Assert.Same(entry, ResettableObjectPool<RingEntry>.Rent());
    }

    /// <summary>
    /// Entry type for <see cref="T1SortedListPool_Return_ReturnsValuesToTheirPool"/>.
    /// </summary>
    private sealed class SortedListValue : IPoolResettable
    {
        /// <summary>
        /// Number of times <see cref="OnReturn"/> ran.
        /// </summary>
        public int OnReturnCalls;

        /// <inheritdoc/>
        public void OnReturn() => OnReturnCalls++;

        /// <inheritdoc/>
        public void OnRent() { }
    }

    /// <summary>
    /// Comparable key type for <see cref="T0SortedListPool_Return_ResetsAndReturnsKeysToTheirPool"/>.
    /// </summary>
    private sealed class SortedListKey : IPoolResettable, IComparable<SortedListKey>
    {
        /// <summary>
        /// Sort ordinal so the SortedList can order keys.
        /// </summary>
        public int Ordinal;
        /// <summary>
        /// Number of times <see cref="OnReturn"/> ran.
        /// </summary>
        public int OnReturnCalls;

        /// <inheritdoc/>
        public int CompareTo(SortedListKey other) => Ordinal.CompareTo(other.Ordinal);

        /// <inheritdoc/>
        public void OnReturn() => OnReturnCalls++;

        /// <inheritdoc/>
        public void OnRent() { }
    }

    /// <summary>
    /// Comparable key type for <see cref="T0T1SortedListPool_Return_ReturnsKeysAndValuesToTheirPools"/>.
    /// </summary>
    private sealed class PairedSortedListKey : IPoolResettable, IComparable<PairedSortedListKey>
    {
        /// <summary>
        /// Sort ordinal so the SortedList can order keys.
        /// </summary>
        public int Ordinal;
        /// <summary>
        /// Number of times <see cref="OnReturn"/> ran.
        /// </summary>
        public int OnReturnCalls;

        /// <inheritdoc/>
        public int CompareTo(PairedSortedListKey other) => Ordinal.CompareTo(other.Ordinal);

        /// <inheritdoc/>
        public void OnReturn() => OnReturnCalls++;

        /// <inheritdoc/>
        public void OnRent() { }
    }

    /// <summary>
    /// Value type paired with <see cref="PairedSortedListKey"/>.
    /// </summary>
    private sealed class PairedSortedListValue : IPoolResettable
    {
        /// <summary>
        /// Number of times <see cref="OnReturn"/> ran.
        /// </summary>
        public int OnReturnCalls;

        /// <inheritdoc/>
        public void OnReturn() => OnReturnCalls++;

        /// <inheritdoc/>
        public void OnRent() { }
    }

    /// <summary>
    /// Key type for <see cref="T0DictionaryPool_Return_ReturnsKeysToTheirPool"/>.
    /// </summary>
    private sealed class DictionaryKey : IPoolResettable
    {
        /// <summary>
        /// Number of times <see cref="OnReturn"/> ran.
        /// </summary>
        public int OnReturnCalls;

        /// <inheritdoc/>
        public void OnReturn() => OnReturnCalls++;

        /// <inheritdoc/>
        public void OnRent() { }
    }

    /// <summary>
    /// Value type for <see cref="T1DictionaryPool_Return_ReturnsValuesToTheirPool"/>.
    /// </summary>
    private sealed class DictionaryValue : IPoolResettable
    {
        /// <summary>
        /// Number of times <see cref="OnReturn"/> ran.
        /// </summary>
        public int OnReturnCalls;

        /// <inheritdoc/>
        public void OnReturn() => OnReturnCalls++;

        /// <inheritdoc/>
        public void OnRent() { }
    }

    /// <summary>
    /// Key type for <see cref="T0T1DictionaryPool_Return_ReturnsKeysAndValuesToTheirPools"/>.
    /// </summary>
    private sealed class PairedDictionaryKey : IPoolResettable
    {
        /// <summary>
        /// Number of times <see cref="OnReturn"/> ran.
        /// </summary>
        public int OnReturnCalls;

        /// <inheritdoc/>
        public void OnReturn() => OnReturnCalls++;

        /// <inheritdoc/>
        public void OnRent() { }
    }

    /// <summary>
    /// Value type paired with <see cref="PairedDictionaryKey"/>.
    /// </summary>
    private sealed class PairedDictionaryValue : IPoolResettable
    {
        /// <summary>
        /// Number of times <see cref="OnReturn"/> ran.
        /// </summary>
        public int OnReturnCalls;

        /// <inheritdoc/>
        public void OnReturn() => OnReturnCalls++;

        /// <inheritdoc/>
        public void OnRent() { }
    }

    /// <summary>
    /// Entry type for <see cref="BoostedQueuePool_Reset_ReturnsEntriesToTheirPool"/>.
    /// </summary>
    private sealed class QueueEntry : IPoolResettable
    {
        /// <summary>
        /// Number of times <see cref="OnReturn"/> ran.
        /// </summary>
        public int OnReturnCalls;

        /// <inheritdoc/>
        public void OnReturn() => OnReturnCalls++;

        /// <inheritdoc/>
        public void OnRent() { }
    }

    /// <summary>
    /// Entry type for <see cref="RingBufferPool_Reset_ReturnsEntriesToTheirPool"/>.
    /// </summary>
    private sealed class RingEntry : IPoolResettable
    {
        /// <summary>
        /// Number of times <see cref="OnReturn"/> ran.
        /// </summary>
        public int OnReturnCalls;

        /// <inheritdoc/>
        public void OnReturn() => OnReturnCalls++;

        /// <inheritdoc/>
        public void OnRent() { }
    }
}
