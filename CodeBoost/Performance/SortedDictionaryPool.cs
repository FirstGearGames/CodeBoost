using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for SortedDictionary collections.
/// </summary>
public static class SortedDictionaryPool<T0, T1>
{
    /// <summary>
    /// Rents a SortedDictionary from the pool.
    /// </summary>
    /// <returns>A cleared SortedDictionary collection.</returns>
    public static SortedDictionary<T0, T1> Rent() => PoolStacks<SortedDictionary<T0, T1>>.Rent();

    /// <summary>
    /// Returns a SortedDictionary to the pool and sets the provided reference to null.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref SortedDictionary<T0, T1> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Returns a SortedDictionary to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(SortedDictionary<T0, T1> value)
    {
        if (value is null)
            return;

        value.Clear();

        PoolStacks<SortedDictionary<T0, T1>>.Return(value);
    }
}
