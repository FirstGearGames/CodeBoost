using System.Collections.Generic;
using CodeBoost.Types;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for BoostedQueue collections.
/// </summary>
public static class BoostedQueuePool<T0>
{
    /// <summary>
    /// Rents a BoostedQueue from the pool.
    /// </summary>
    /// <returns>A cleared BoostedQueue collection.</returns>
    public static BoostedQueue<T0> Rent() => PoolStacks<BoostedQueue<T0>>.Rent();

    /// <summary>
    /// Returns a BoostedQueue to the pool and sets the provided reference to null.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref BoostedQueue<T0> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Returns a BoostedQueue to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(BoostedQueue<T0> value)
    {
        if (value is null)
            return;

        value.Clear();

        PoolStacks<BoostedQueue<T0>>.Return(value);
    }
}
