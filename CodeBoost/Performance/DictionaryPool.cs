using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for Dictionary collections.
/// </summary>
public static class DictionaryPool<T0, T1>
{
    /// <summary>
    /// Pools kept per comparer, so a Dictionary rented with a custom comparer is only ever handed back to a caller that asked for the same comparer. Callers that share one comparer instance, the usual case of a singleton, share one stack. Guarded by its own lock and touched only at collection setup and teardown, never in a hot path.
    /// </summary>
    private static readonly Dictionary<IEqualityComparer<T0>, Stack<Dictionary<T0, T1>>> ComparerStacks = new();

    /// <summary>
    /// Maximum number of entries kept in one comparer's stack.
    /// </summary>
    private const int MaximumComparerStackSize = 200;

    /// <summary>
    /// Rents a Dictionary from the pool.
    /// </summary>
    /// <returns>A cleared Dictionary collection.</returns>
    public static Dictionary<T0, T1> Rent() => PoolStacks<Dictionary<T0, T1>>.Rent();

    /// <summary>
    /// Rents a Dictionary that uses the supplied comparer, pooled separately from the default-comparer ones so the comparer is never mixed.
    /// </summary>
    /// <param name = "comparer"> The comparer the rented Dictionary must use. </param>
    /// <returns>A cleared Dictionary that uses <paramref name = "comparer"/>.</returns>
    public static Dictionary<T0, T1> Rent(IEqualityComparer<T0> comparer)
    {
        lock (ComparerStacks)
        {
            if (ComparerStacks.TryGetValue(comparer, out Stack<Dictionary<T0, T1>> stack) && stack.TryPop(out Dictionary<T0, T1> result))
                return result;
        }

        return new(comparer);
    }

    /// <summary>
    /// Returns a Dictionary to the pool and sets the provided reference to null.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref Dictionary<T0, T1> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Returns a comparer-rented Dictionary to its comparer-specific pool and sets the provided reference to null.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    /// <param name = "comparer"> The comparer the Dictionary was rented with. </param>
    public static void ReturnAndNullifyReference(ref Dictionary<T0, T1> value, IEqualityComparer<T0> comparer)
    {
        Return(value, comparer);

        value = null;
    }

    /// <summary>
    /// Returns a Dictionary to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(Dictionary<T0, T1> value)
    {
        if (value is null)
            return;

        value.Clear();

        PoolStacks<Dictionary<T0, T1>>.Return(value);
    }

    /// <summary>
    /// Returns a Dictionary that was rented with a comparer to that comparer's pool. Clearing keeps the comparer, so the reused instance stays correct.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    /// <param name = "comparer"> The comparer the Dictionary was rented with. </param>
    public static void Return(Dictionary<T0, T1> value, IEqualityComparer<T0> comparer)
    {
        if (value is null)
            return;

        value.Clear();

        lock (ComparerStacks)
        {
            if (!ComparerStacks.TryGetValue(comparer, out Stack<Dictionary<T0, T1>> stack))
            {
                stack = new();
                ComparerStacks.Add(comparer, stack);
            }

            if (stack.Count < MaximumComparerStackSize)
                stack.Push(value);
        }
    }
}
