using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using CodeBoost.Extensions;
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

    /// <summary>
    /// The leak report counts and names only the instances the caller leaked, not the list it builds the report in.
    /// </summary>
    [Fact]
    public void ThrowIfAnyRented_WithOneLeak_ReportsOnlyThatLeak()
    {
        ObjectPool<TrackedThing>.Rent();

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(PoolTracker.ThrowIfAnyRented);

        Assert.Equal($"1 pooled instances were never returned: {nameof(TrackedThing)}.", exception.Message);
    }

    /// <summary>
    /// An instance returned while both of its pool's stacks are full is discarded, and that still ends its rental.
    /// </summary>
    [Fact]
    public void ObjectPool_ReturnedPastCapacity_IsNotCountedAsRented()
    {
        //More than both stacks hold together, so the last returns are discarded.
        const int RentalCount = 10_000;

        CapacityThing[] rented = new CapacityThing[RentalCount];

        for (int index = 0; index < RentalCount; index++)
            rented[index] = ObjectPool<CapacityThing>.Rent();

        foreach (CapacityThing thing in rented)
            ObjectPool<CapacityThing>.Return(thing);

        Assert.Equal(0, PoolTracker.GetRentedCount());
        PoolTracker.ThrowIfAnyRented();
    }

    /// <summary>
    /// Instances a thread cached in its own stack are collected once the thread exits, as they are without tracking; the tracker must not be what keeps them alive.
    /// </summary>
    [Fact]
    public void ObjectPool_InstancesCachedByAnExitedThread_AreCollected()
    {
        WeakReference[] cached = RentAndReturnOnRetiringThread();

        CollectEverything();

        Assert.Equal(0, CountAlive(cached));
    }

    /// <summary>
    /// An array longer than the pool keeps is left to the collector on return; the tracker must not be what keeps it alive.
    /// </summary>
    [Fact]
    public void TrackedArrayPool_ArrayLongerThanThePoolKeeps_IsCollectedAfterReturn()
    {
        WeakReference[] returned = RentAndReturnArrayLongerThanThePoolKeeps();

        CollectEverything();

        Assert.Equal(0, CountAlive(returned));
    }

    /// <summary>
    /// Rents and returns instances on a thread that then exits.
    /// </summary>
    /// <returns>A weak reference to each instance the thread cached.</returns>
    /// <remarks>Not inlined, so no reference to an instance survives in the caller's frame.</remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] RentAndReturnOnRetiringThread()
    {
        WeakReference[] cached = new WeakReference[64];

        Thread retiringThread = new(() =>
        {
            ThreadCachedThing[] rented = new ThreadCachedThing[cached.Length];

            for (int index = 0; index < rented.Length; index++)
            {
                rented[index] = ObjectPool<ThreadCachedThing>.Rent();
                cached[index] = new(rented[index]);
            }

            foreach (ThreadCachedThing thing in rented)
                ObjectPool<ThreadCachedThing>.Return(thing);
        });

        retiringThread.Start();
        Assert.True(retiringThread.Join(millisecondsTimeout: 10_000));

        return cached;
    }

    /// <summary>
    /// Rents and returns one array longer than the pool keeps.
    /// </summary>
    /// <returns>A weak reference to the returned array.</returns>
    /// <remarks>Not inlined, so no reference to the array survives in the caller's frame.</remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] RentAndReturnArrayLongerThanThePoolKeeps()
    {
        //One element past the longest array the pool keeps, 1 << 24.
        byte[] array = TrackedArrayPool<byte>.Rent((1 << 24) + 1);
        WeakReference[] returned = [new(array)];

        TrackedArrayPool<byte>.Return(array);

        return returned;
    }

    /// <summary>
    /// Runs a full collection, finalizers included, and then collects what the finalizers released.
    /// </summary>
    private static void CollectEverything()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    /// <summary>
    /// Counts the weak references whose target is still alive.
    /// </summary>
    /// <param name="references">The references to check.</param>
    /// <returns>The number still alive.</returns>
    private static int CountAlive(WeakReference[] references)
    {
        int alive = 0;

        foreach (WeakReference reference in references)
            if (reference.IsAlive)
                alive++;

        return alive;
    }

    /// <summary>
    /// A string UTF-8 cannot encode, such as a lone surrogate, throws from the conversion with its buffer and its encoding back in their
    /// pools. Both were rented before the encoder threw and nothing returned them, so every such string leaked a tracked buffer.
    /// </summary>
    [Fact]
    public void ToBytesNonAllocated_EncodingThrows_ReturnsTheBufferAndTheEncoding()
    {
        UTF8Encoding probeEncoding = Utf8EncodingPool.Rent();
        Utf8EncodingPool.Return(probeEncoding);

        Assert.Throws<EncoderFallbackException>(() => "\uD800".ToBytesNonAllocated(out _));

        Assert.Equal(0, PoolTracker.GetRentedCount());
        UTF8Encoding nextEncoding = Utf8EncodingPool.Rent();
        Utf8EncodingPool.Return(nextEncoding);
        Assert.True(ReferenceEquals(probeEncoding, nextEncoding), "the encoding rented by the failed conversion was not returned");
    }

    /// <summary>
    /// Bytes that are not UTF-8 throw from the conversion with its encoding back in the pool.
    /// </summary>
    [Fact]
    public void ToEncodedString_DecodingThrows_ReturnsTheEncoding()
    {
        UTF8Encoding probeEncoding = Utf8EncodingPool.Rent();
        Utf8EncodingPool.Return(probeEncoding);

        Assert.Throws<DecoderFallbackException>(() => new byte[] { 0xFF }.ToEncodedString(offset: 0, count: 1));

        UTF8Encoding nextEncoding = Utf8EncodingPool.Rent();
        Utf8EncodingPool.Return(nextEncoding);
        Assert.True(ReferenceEquals(probeEncoding, nextEncoding), "the encoding rented by the failed conversion was not returned");
    }

    private sealed class TrackedThing
    {
    }

    /// <summary>
    /// Used only by <see cref="ObjectPool_ReturnedPastCapacity_IsNotCountedAsRented"/>, so its stacks start empty.
    /// </summary>
    private sealed class CapacityThing
    {
    }

    /// <summary>
    /// Used only by <see cref="ObjectPool_InstancesCachedByAnExitedThread_AreCollected"/>, so its stacks start empty.
    /// </summary>
    private sealed class ThreadCachedThing
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
