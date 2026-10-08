using System;
using System.Collections.Generic;
using System.Threading;
using CodeBoost.Extensions;

namespace CodeBoost.Performance;

/// <summary>
/// Holds the per-thread and global stacks behind every CodeBoost pool, so the stack logic and the double-return check live in one place.
/// </summary>
/// <remarks>This is internal so that only CodeBoost's own pools can place an instance in it, which keeps a collection from reaching the stacks without being cleared.</remarks>
internal static class PoolStacks<T0> where T0 : new()
{
    /// <summary>
    /// The stack for the ThreadLocal object.
    /// </summary>
    private static readonly ThreadLocal<ThreadLocalStackWrapper<T0>> Wrapper;
    /// <summary>
    /// The stack for the global object.
    /// </summary>
    private static readonly Stack<T0> GlobalStack = [];
    /// <summary>
    /// Maximum number of entries allowed in the global stack.
    /// </summary>
    /// <remarks>Sized for churn rather than for an average: a return past the cap drops the instance, so a workload that returns more per frame than the stacks hold rebuilds the excess on its next rents. Spawning and despawning hundreds of networked objects a tick returns several hundred of a type at once.</remarks>
    private const int MaximumGlobalStackSize = 4096;
    /// <summary>
    /// Maximum number of entries allowed in the ThreadLocal stack.
    /// </summary>
    /// <remarks>Sized for the same churn as <see cref="MaximumGlobalStackSize"/>; the thread that returns is usually the thread that rents next, so most reuse happens here.</remarks>
    private const int MaximumThreadLocalStackSize = 1024;

    static PoolStacks()
    {
        //A value type is copied on every pass, so pooling one reuses nothing and a returned copy cannot be told apart from another.
        if (typeof(T0).IsValueType)
            throw new InvalidOperationException($"[{typeof(T0).Name}] is a value type; only reference types can be pooled.");

        Wrapper = new(valueFactory: () => new(), trackAllValues: false);
    }

    /// <summary>
    /// Rents an instance from the stacks, or creates one when both are empty.
    /// </summary>
    /// <returns>A pooled or new instance.</returns>
    internal static T0 Rent()
    {
        Stack<T0> localStack = Wrapper.Value.LocalStack;
        if (!localStack.TryPop(out T0 result))
        {
            lock (GlobalStack)
            {
                if (!GlobalStack.TryPop(out result))
                    result = new();
            }
        }

        if (PoolTracker.TrackingEnabled)
            PoolTracker.RecordRented(result);

        return result;
    }

    /// <summary>
    /// Places an instance in the stacks, or discards it when both are at capacity.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name="value">The instance to place.</param>
    internal static void Return(T0 value)
    {
        if (value is null)
            return;

        Stack<T0> localStack = Wrapper.Value.LocalStack;
        if (localStack.Count < MaximumThreadLocalStackSize)
        {
            if (PoolTracker.TrackingEnabled)
                PoolTracker.RecordPooled(value);

            localStack.Push(value);
            return;
        }

        lock (GlobalStack)
        {
            if (GlobalStack.Count < MaximumGlobalStackSize)
            {
                if (PoolTracker.TrackingEnabled)
                    PoolTracker.RecordPooled(value);

                GlobalStack.Push(value);
            }
        }

        //If here both stacks are at capacity.
    }
}
