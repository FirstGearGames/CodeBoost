using System.Collections.Generic;
using System.Threading;
using CodeBoost.Extensions;
using CodeBoost.Logging;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for HashSet collections.
/// </summary>
public static class HashSetPool<T0>
{
    /// <summary>
    /// The stack for the ThreadLocal HashSet.
    /// </summary>
    private static readonly ThreadLocal<ThreadLocalStackWrapper<HashSet<T0>>> Wrapper;
    /// <summary>
    /// The stack for the global HashSet.
    /// </summary>
    private static readonly Stack<HashSet<T0>> GlobalStack = [];
    /// <summary>
    /// Maximum number of entries allowed in the global stack.
    /// </summary>
    private const int MaximumGlobalStackSize = 200;
    /// <summary>
    /// Maximum number of entries allowed in the ThreadLocal stack.
    /// </summary>
    private const int MaximumThreadLocalStackSize = 100;

    static HashSetPool()
    {
        // if (typeof(IPoolResettable).IsAssignableFrom(typeof(T0)))
        // {
        //     Logger.LogError(typeof(HashSetPool<>), $"[{typeof(T0).Name}] implements IPoolResettable; use the Resettable pool instead.");
        //     return;
        // }
        
        Wrapper = new(valueFactory: () => new(Flush), trackAllValues: false);
    }

    /// <summary>
    /// Rents a HashSet from the pool.
    /// </summary>
    /// <returns>A cleared HashSet collection.</returns>
    public static HashSet<T0> Rent()
    {
        HashSet<T0> result;

        if (Wrapper.Value.LocalStack.TryPop(out result))
            return result;

        lock (GlobalStack)
        {
            if (GlobalStack.TryPop(out result))
                return result;
        }

        return new();
    }

    /// <summary>
    /// Returns a HashSet to the pool and sets the provided reference to null.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref HashSet<T0>? value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Returns a HashSet to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(HashSet<T0>? value)
    {
        if (value is null)
            return;

        value.Clear();

        if (Wrapper.Value.LocalStack.Count < MaximumThreadLocalStackSize)
        {
            Wrapper.Value.LocalStack.Push(value);
            return;
        }

        lock (GlobalStack)
        {
            if (GlobalStack.Count < MaximumGlobalStackSize)
                GlobalStack.Push(value);
        }

        //If here both stacks are at capacity.
    }

    /// <summary>
    /// Flushes the ThreadLocal HashSet stack into the global stack.
    /// </summary>
    private static void Flush(Stack<HashSet<T0>> localStack)
    {
        if (localStack.Count == 0)
            return;

        lock (GlobalStack)
        {
            while (localStack.TryPop(out HashSet<T0>? item))
            {
                if (GlobalStack.Count < MaximumGlobalStackSize)
                    GlobalStack.Push(item);
                else
                    break;
            }
        }
    }
}