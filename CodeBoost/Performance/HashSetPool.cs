using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for HashSet collections.
/// </summary>
public static class HashSetPool<T0>
{
    /// <summary>
    /// Pools kept per comparer, so a HashSet rented with a custom comparer is only ever handed back to a caller that asked for the same comparer. Callers that share one comparer instance, the usual case of a singleton, share one stack. Guarded by its own lock and touched only at collection setup and teardown, never in a hot path.
    /// </summary>
    private static readonly Dictionary<IEqualityComparer<T0>, Stack<HashSet<T0>>> ComparerStacks = new();

    /// <summary>
    /// Maximum number of entries kept in one comparer's stack.
    /// </summary>
    private const int MaximumComparerStackSize = 200;

    /// <summary>
    /// Rents a HashSet from the pool.
    /// </summary>
    /// <returns>A cleared HashSet collection.</returns>
    public static HashSet<T0> Rent() => PoolStacks<HashSet<T0>>.Rent();

    /// <summary>
    /// Rents a HashSet that uses the supplied comparer, pooled separately from the default-comparer ones so the comparer is never mixed.
    /// </summary>
    /// <param name = "comparer"> The comparer the rented HashSet must use. </param>
    /// <returns>A cleared HashSet that uses <paramref name = "comparer"/>.</returns>
    public static HashSet<T0> Rent(IEqualityComparer<T0> comparer)
    {
        lock (ComparerStacks)
        {
            if (ComparerStacks.TryGetValue(comparer, out Stack<HashSet<T0>> stack) && stack.TryPop(out HashSet<T0> result))
                return result;
        }

        return new(comparer);
    }

    /// <summary>
    /// Returns a HashSet to the pool and sets the provided reference to null.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref HashSet<T0> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Returns a comparer-rented HashSet to its comparer-specific pool and sets the provided reference to null.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    /// <param name = "comparer"> The comparer the HashSet was rented with. </param>
    public static void ReturnAndNullifyReference(ref HashSet<T0> value, IEqualityComparer<T0> comparer)
    {
        Return(value, comparer);

        value = null;
    }

    /// <summary>
    /// Returns a HashSet to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(HashSet<T0> value)
    {
        if (value is null)
            return;

        value.Clear();

        PoolStacks<HashSet<T0>>.Return(value);
    }

    /// <summary>
    /// Returns a HashSet that was rented with a comparer to that comparer's pool. Clearing keeps the comparer, so the reused instance stays correct.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    /// <param name = "comparer"> The comparer the HashSet was rented with. </param>
    public static void Return(HashSet<T0> value, IEqualityComparer<T0> comparer)
    {
        if (value is null)
            return;

        value.Clear();

        lock (ComparerStacks)
        {
            if (!ComparerStacks.TryGetValue(comparer, out Stack<HashSet<T0>> stack))
            {
                stack = new();
                ComparerStacks.Add(comparer, stack);
            }

            if (stack.Count < MaximumComparerStackSize)
                stack.Push(value);
        }
    }
}
