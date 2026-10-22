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
    /// The entry held in a returned List re-enters its object pool. Reset ran each entry's OnRent instead of OnReturn and then
    /// dropped it, so the entry was never reset and its pool never refilled.
    /// </summary>
    [Fact]
    public void ListPool_Return_ReturnsEntriesToTheirPool()
    {
        ListEntry entry = ResettableObjectPool<ListEntry>.Rent();
        List<ListEntry> list = ResettableListPool<ListEntry>.Rent();
        list.Add(entry);

        ResettableListPool<ListEntry>.Return(list);

        Assert.Equal(1, entry.OnReturnCalls);
        Assert.Same(entry, ResettableObjectPool<ListEntry>.Rent());
    }

    /// <summary>
    /// The entry held in a returned HashSet re-enters its object pool.
    /// </summary>
    [Fact]
    public void HashSetPool_Return_ReturnsEntriesToTheirPool()
    {
        HashSetEntry entry = ResettableObjectPool<HashSetEntry>.Rent();
        HashSet<HashSetEntry> hashSet = ResettableHashSetPool<HashSetEntry>.Rent();
        hashSet.Add(entry);

        ResettableHashSetPool<HashSetEntry>.Return(hashSet);

        Assert.Equal(1, entry.OnReturnCalls);
        Assert.Same(entry, ResettableObjectPool<HashSetEntry>.Rent());
    }

    /// <summary>
    /// The entry held in a returned Queue re-enters its object pool.
    /// </summary>
    [Fact]
    public void QueuePool_Return_ReturnsEntriesToTheirPool()
    {
        StandardQueueEntry entry = ResettableObjectPool<StandardQueueEntry>.Rent();
        Queue<StandardQueueEntry> queue = ResettableQueuePool<StandardQueueEntry>.Rent();
        queue.Enqueue(entry);

        ResettableQueuePool<StandardQueueEntry>.Return(queue);

        Assert.Equal(1, entry.OnReturnCalls);
        Assert.Same(entry, ResettableObjectPool<StandardQueueEntry>.Rent());
    }

    /// <summary>
    /// The value held in a returned SortedDictionary re-enters its object pool. Return only cleared the dictionary, so its values
    /// were neither reset nor pooled.
    /// </summary>
    [Fact]
    public void T1SortedDictionaryPool_Return_ReturnsValuesToTheirPool()
    {
        SortedDictionaryValue value = ResettableObjectPool<SortedDictionaryValue>.Rent();
        SortedDictionary<uint, SortedDictionaryValue> dictionary = ResettableT1SortedDictionaryPool<uint, SortedDictionaryValue>.Rent();
        dictionary.Add(1, value);

        ResettableT1SortedDictionaryPool<uint, SortedDictionaryValue>.Return(dictionary);

        Assert.Equal(1, value.OnReturnCalls);
        Assert.Same(value, ResettableObjectPool<SortedDictionaryValue>.Rent());
    }

    /// <summary>
    /// Resetting a collection without returning it sends its entries back to their pools, so the collection must let go of them
    /// too; otherwise its owner still holds instances the pool hands to the next renter, and two owners share one entry.
    /// </summary>
    [Fact]
    public void Reset_EveryCollectionPool_EmptiesTheCollection()
    {
        List<ResetEntry> list = ResettableListPool<ResetEntry>.Rent();
        list.Add(new());
        ResettableListPool<ResetEntry>.Reset(list);
        Assert.Empty(list);

        HashSet<ResetEntry> hashSet = ResettableHashSetPool<ResetEntry>.Rent();
        hashSet.Add(new());
        ResettableHashSetPool<ResetEntry>.Reset(hashSet);
        Assert.Empty(hashSet);

        Queue<ResetEntry> queue = ResettableQueuePool<ResetEntry>.Rent();
        queue.Enqueue(new());
        ResettableQueuePool<ResetEntry>.Reset(queue);
        Assert.Empty(queue);

        Dictionary<ResetEntry, uint> keyDictionary = ResettableT0DictionaryPool<ResetEntry, uint>.Rent();
        keyDictionary.Add(new(), 1);
        ResettableT0DictionaryPool<ResetEntry, uint>.Reset(keyDictionary);
        Assert.Empty(keyDictionary);

        Dictionary<uint, ResetEntry> valueDictionary = ResettableT1DictionaryPool<uint, ResetEntry>.Rent();
        valueDictionary.Add(1, new());
        ResettableT1DictionaryPool<uint, ResetEntry>.Reset(valueDictionary);
        Assert.Empty(valueDictionary);

        Dictionary<ResetEntry, ResetEntry> pairedDictionary = ResettableT0T1DictionaryPool<ResetEntry, ResetEntry>.Rent();
        pairedDictionary.Add(new(), new());
        ResettableT0T1DictionaryPool<ResetEntry, ResetEntry>.Reset(pairedDictionary);
        Assert.Empty(pairedDictionary);

        SortedList<ResetKey, uint> keySortedList = ResettableT0SortedListPool<ResetKey, uint>.Rent();
        keySortedList.Add(new(), 1);
        ResettableT0SortedListPool<ResetKey, uint>.Reset(keySortedList);
        Assert.Empty(keySortedList);

        SortedList<uint, ResetEntry> valueSortedList = ResettableT1SortedListPool<uint, ResetEntry>.Rent();
        valueSortedList.Add(1, new());
        ResettableT1SortedListPool<uint, ResetEntry>.Reset(valueSortedList);
        Assert.Empty(valueSortedList);

        SortedList<ResetKey, ResetEntry> pairedSortedList = ResettableT0T1SortedListPool<ResetKey, ResetEntry>.Rent();
        pairedSortedList.Add(new(), new());
        ResettableT0T1SortedListPool<ResetKey, ResetEntry>.Reset(pairedSortedList);
        Assert.Empty(pairedSortedList);

        SortedDictionary<uint, ResetEntry> valueSortedDictionary = ResettableT1SortedDictionaryPool<uint, ResetEntry>.Rent();
        valueSortedDictionary.Add(1, new());
        ResettableT1SortedDictionaryPool<uint, ResetEntry>.Reset(valueSortedDictionary);
        Assert.Empty(valueSortedDictionary);
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

    /// <summary>
    /// Entry type for <see cref="ListPool_Return_ReturnsEntriesToTheirPool"/>.
    /// </summary>
    private sealed class ListEntry : IPoolResettable
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
    /// Entry type for <see cref="HashSetPool_Return_ReturnsEntriesToTheirPool"/>.
    /// </summary>
    private sealed class HashSetEntry : IPoolResettable
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
    /// Entry type for <see cref="QueuePool_Return_ReturnsEntriesToTheirPool"/>.
    /// </summary>
    private sealed class StandardQueueEntry : IPoolResettable
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
    /// Value type for <see cref="T1SortedDictionaryPool_Return_ReturnsValuesToTheirPool"/>.
    /// </summary>
    private sealed class SortedDictionaryValue : IPoolResettable
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
    /// Entry type for <see cref="Reset_EveryCollectionPool_EmptiesTheCollection"/>.
    /// </summary>
    private sealed class ResetEntry : IPoolResettable
    {
        /// <inheritdoc/>
        public void OnReturn() { }

        /// <inheritdoc/>
        public void OnRent() { }
    }

    /// <summary>
    /// Sorted-list key type for <see cref="Reset_EveryCollectionPool_EmptiesTheCollection"/>; each test collection holds one key, so every key compares equal.
    /// </summary>
    private sealed class ResetKey : IPoolResettable, IComparable<ResetKey>
    {
        /// <inheritdoc/>
        public int CompareTo(ResetKey? other) => 0;

        /// <inheritdoc/>
        public void OnReturn() { }

        /// <inheritdoc/>
        public void OnRent() { }
    }
}
