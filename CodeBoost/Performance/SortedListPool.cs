using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for SortedList collections.
/// </summary>
public static class SortedListPool<T0, T1>
{
    /// <summary>
    /// Rents a SortedList from the pool.
    /// </summary>
    /// <returns>A cleared SortedList collection.</returns>
    public static SortedList<T0, T1> Rent() => PoolStacks<SortedList<T0, T1>>.Rent();

    /// <summary>
    /// Returns a SortedList to the pool and sets the provided reference to null.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref SortedList<T0, T1> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Returns a SortedList to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(SortedList<T0, T1> value)
    {
        if (value is null)
            return;

        value.Clear();

        PoolStacks<SortedList<T0, T1>>.Return(value);
    }
}
