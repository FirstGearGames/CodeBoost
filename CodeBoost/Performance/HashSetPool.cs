using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for HashSet collections.
/// </summary>
public static class HashSetPool<T0>
{
    /// <summary>
    /// Rents a HashSet from the pool.
    /// </summary>
    /// <returns>A cleared HashSet collection.</returns>
    public static HashSet<T0> Rent() => PoolStacks<HashSet<T0>>.Rent();

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
}
