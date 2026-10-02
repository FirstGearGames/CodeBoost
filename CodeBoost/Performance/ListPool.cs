using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for List collections.
/// </summary>
public static class ListPool<T0>
{
    /// <summary>
    /// Rents a List from the pool.
    /// </summary>
    /// <returns>A cleared List collection.</returns>
    public static List<T0> Rent() => PoolStacks<List<T0>>.Rent();

    /// <summary>
    /// Returns a List to the pool and sets the provided reference to null.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref List<T0> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Returns a List to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(List<T0> value)
    {
        if (value is null)
            return;

        value.Clear();

        PoolStacks<List<T0>>.Return(value);
    }
}
