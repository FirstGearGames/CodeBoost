using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using CodeBoost.Performance;
using Xunit;

namespace CodeBoost.Tests;

/// <summary>
/// Coverage for the pools not harvesting a retiring thread's cached entries from a finalizer, and for pooling still working across
/// threads without it.
/// </summary>
/// <remarks>
/// <para>
/// The harvest existed and was unsound. A per-thread stack reaches its finalizer only once it is unreachable, which means the entries
/// it holds are unreachable too and are themselves eligible for finalization; finalization order is undefined, so any of them may
/// already have run its own finalizer. Pushing those into the shared stack resurrected them, and the pool then handed a later caller an
/// object that had already torn itself down. A pooled <see cref="System.Timers.Timer"/> came back marked disposed and threw
/// <see cref="ObjectDisposedException"/> the moment it was started, from a stack trace pointing nowhere near the pool.
/// </para>
/// <para>
/// It is pinned structurally rather than behaviourally because the failure needs a thread to retire, a collection, and an unfavourable
/// finalization order all at once, which no test can arrange deterministically. The regression to guard is someone reinstating the
/// harvest as an optimisation, and that is exactly what the finalizer's absence catches.
/// </para>
/// </remarks>
public class PoolFinalizerHarvestTests
{
    /// <summary>
    /// The per-thread stack holder declares no finalizer, so nothing a thread cached can be pushed back into a shared stack after it
    /// has become garbage.
    /// </summary>
    [Fact]
    public void ThreadLocalStackWrapper_DeclaresNoFinalizer()
    {
        // Never null: every type inherits object's own finalizer, so the declaring type is what says whether this one overrode it.
        MethodInfo finalizer = typeof(ThreadLocalStackWrapper<object>).GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic)!;

        Assert.Equal(typeof(object), finalizer.DeclaringType);
    }

    /// <summary>
    /// No pool passes a finalize callback to the holder, which is the other half of the same guarantee: the holder cannot harvest, and
    /// nothing asks it to.
    /// </summary>
    [Fact]
    public void ThreadLocalStackWrapper_TakesNoFinalizeCallback()
    {
        ConstructorInfo[] constructors = typeof(ThreadLocalStackWrapper<object>).GetConstructors();

        foreach (ConstructorInfo constructor in constructors)
            Assert.Empty(constructor.GetParameters());
    }

    /// <summary>
    /// A pool still serves correctly after a thread that used it has exited, which is the case the harvest was there to optimise.
    /// </summary>
    /// <remarks>Entries cached on the retired thread are collected rather than recycled; the pool simply constructs replacements, which is what its <c>new()</c> constraint is for.</remarks>
    [Fact]
    public void Pool_AfterAThreadThatUsedItHasExited_StillRentsAndReturns()
    {
        Thread retiringThread = new(() =>
        {
            for (int entryIndex = 0; entryIndex < 8; entryIndex++)
            {
                List<int> rented = ListPool<int>.Rent();
                rented.Add(entryIndex);
                ListPool<int>.Return(rented);
            }
        });

        retiringThread.Start();
        Assert.True(retiringThread.Join(millisecondsTimeout: 10_000));

        /* Collected and finalized with the thread gone, which is the moment the harvest used to run. Nothing should reach the shared
         * stack from it, and the pool must be unharmed either way. */
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        List<int> afterRetirement = ListPool<int>.Rent();

        Assert.Empty(afterRetirement);

        afterRetirement.Add(1);
        ListPool<int>.Return(afterRetirement);

        List<int> reused = ListPool<int>.Rent();

        Assert.Empty(reused);

        ListPool<int>.Return(reused);
    }
}
