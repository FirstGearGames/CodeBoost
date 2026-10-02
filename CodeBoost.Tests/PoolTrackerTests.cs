using System;
using System.Collections.Generic;
using CodeBoost.Performance;
using Xunit;

namespace CodeBoost.Tests;

/// <summary>
/// Runs the tracker tests alone, because the tracking is process-wide and the leak counts would otherwise include other tests' rentals.
/// </summary>
[CollectionDefinition(nameof(PoolTrackerTests), DisableParallelization = true)]
public class PoolTrackerCollection
{
}

[Collection(nameof(PoolTrackerTests))]
public class PoolTrackerTests : IDisposable
{
    public PoolTrackerTests()
    {
        PoolTracker.EnableTracking();
    }

    public void Dispose()
    {
        PoolTracker.DisableTracking();
    }

    [Fact]
    public void ObjectPool_ReturnTwice_ThrowsOnSecondReturn()
    {
        TrackedThing thing = ObjectPool<TrackedThing>.Rent();
        ObjectPool<TrackedThing>.Return(thing);

        Assert.Throws<InvalidOperationException>(() => ObjectPool<TrackedThing>.Return(thing));
    }

    [Fact]
    public void ObjectPool_ReturnTwice_DoesNotHandTheSameInstanceToTwoRenters()
    {
        TrackedThing thing = ObjectPool<TrackedThing>.Rent();
        ObjectPool<TrackedThing>.Return(thing);
        Assert.Throws<InvalidOperationException>(() => ObjectPool<TrackedThing>.Return(thing));

        TrackedThing first = ObjectPool<TrackedThing>.Rent();
        TrackedThing second = ObjectPool<TrackedThing>.Rent();

        Assert.NotSame(first, second);

        ObjectPool<TrackedThing>.Return(first);
        ObjectPool<TrackedThing>.Return(second);
    }

    [Fact]
    public void ObjectPool_ReturnNeverRented_IsAccepted()
    {
        ObjectPool<TrackedThing>.Return(new());
    }

    [Fact]
    public void ObjectPool_TypeOverridingEquality_IsTrackedPerInstance()
    {
        EqualThing first = ObjectPool<EqualThing>.Rent();
        EqualThing second = ObjectPool<EqualThing>.Rent();

        ObjectPool<EqualThing>.Return(first);
        ObjectPool<EqualThing>.Return(second);
    }

    [Fact]
    public void ObjectPool_Collection_Throws()
    {
        TypeInitializationException exception = Assert.Throws<TypeInitializationException>(() => ObjectPool<List<int>>.Rent());

        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public void ResettableObjectPool_ReturnTwice_ThrowsOnSecondReturn()
    {
        ResettableThing thing = ResettableObjectPool<ResettableThing>.Rent();
        ResettableObjectPool<ResettableThing>.Return(thing);

        Assert.Throws<InvalidOperationException>(() => ResettableObjectPool<ResettableThing>.Return(thing));
    }

    [Fact]
    public void ListPool_ReturnTwice_ThrowsOnSecondReturn()
    {
        List<int> list = ListPool<int>.Rent();
        ListPool<int>.Return(list);

        Assert.Throws<InvalidOperationException>(() => ListPool<int>.Return(list));
    }

    [Fact]
    public void DictionaryPool_ReturnTwice_ThrowsOnSecondReturn()
    {
        Dictionary<int, string> dictionary = DictionaryPool<int, string>.Rent();
        DictionaryPool<int, string>.Return(dictionary);

        Assert.Throws<InvalidOperationException>(() => DictionaryPool<int, string>.Return(dictionary));
    }

    [Fact]
    public void TrackedArrayPool_ReturnTwice_ThrowsOnSecondReturn()
    {
        byte[] array = TrackedArrayPool<byte>.Rent(64);
        TrackedArrayPool<byte>.Return(array);

        Assert.Throws<InvalidOperationException>(() => TrackedArrayPool<byte>.Return(array));
    }

    [Fact]
    public void TrackedArrayPool_ReturnNeverRented_IsAccepted()
    {
        TrackedArrayPool<int>.Return(new int[16]);
    }

    [Fact]
    public void TrackedArrayPool_EmptyRentedTwice_ReturnsWithoutThrowing()
    {
        int[] first = TrackedArrayPool<int>.Rent(0);
        int[] second = TrackedArrayPool<int>.Rent(0);

        TrackedArrayPool<int>.Return(first);
        TrackedArrayPool<int>.Return(second);

        Assert.Equal(0, PoolTracker.GetRentedCount());
    }

    [Fact]
    public void TrackedArrayPool_ResizeWithoutClearing_CopiesAndTracksBothArrays()
    {
        byte[] array = TrackedArrayPool<byte>.Rent(16);
        byte[] oldArray = array;
        array[3] = 7;

        TrackedArrayPool<byte>.ResizeWithoutClearing(ref array, 4096);

        Assert.True(array.Length >= 4096);
        Assert.Equal(7, array[3]);
        Assert.Equal(1, PoolTracker.GetRentedCount());
        Assert.Throws<InvalidOperationException>(() => TrackedArrayPool<byte>.Return(oldArray));

        TrackedArrayPool<byte>.Return(array);
        PoolTracker.ThrowIfAnyRented();
    }

    [Fact]
    public void ThrowIfAnyRented_AfterEveryReturn_DoesNotThrow()
    {
        int[] ints = TrackedArrayPool<int>.Rent(32);
        string[] strings = TrackedArrayPool<string>.Rent(8);
        TrackedThing thing = ObjectPool<TrackedThing>.Rent();
        TrackedArrayPool<int>.Return(ints);
        TrackedArrayPool<string>.ReturnAndNullifyReference(ref strings, clearArray: true);
        ObjectPool<TrackedThing>.Return(thing);

        Assert.Null(strings);
        Assert.Equal(0, PoolTracker.GetRentedCount());
        PoolTracker.ThrowIfAnyRented();
    }

    [Fact]
    public void ThrowIfAnyRented_WithLeaksAcrossPools_NamesEachLeak()
    {
        TrackedArrayPool<int>.Rent(32);
        ObjectPool<TrackedThing>.Rent();
        ListPool<int>.Rent();

        Assert.Equal(3, PoolTracker.GetRentedCount());
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(PoolTracker.ThrowIfAnyRented);
        Assert.Contains("Int32[", exception.Message);
        Assert.Contains(nameof(TrackedThing), exception.Message);
        Assert.Contains("List`1", exception.Message);
    }

    [Fact]
    public void ReturnTwice_WhileTrackingDisabled_PassesThrough()
    {
        PoolTracker.DisableTracking();

        TrackedThing thing = ObjectPool<TrackedThing>.Rent();
        ObjectPool<TrackedThing>.Return(thing);

        Assert.False(PoolTracker.TrackingEnabled);
        Assert.Equal(0, PoolTracker.GetRentedCount());
    }

    private sealed class TrackedThing
    {
    }

    /// <summary>
    /// Every instance equals every other, so tracking by equality would mistake the second return for a double return.
    /// </summary>
    private sealed class EqualThing
    {
        public override bool Equals(object? obj) => obj is EqualThing;

        public override int GetHashCode() => 0;
    }

    private sealed class ResettableThing : IPoolResettable
    {
        public void OnReturn()
        {
        }

        public void OnRent()
        {
        }
    }
}
