using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for a SortedList whose keys and values are both resettable.
/// </summary>
public static class ResettableT0T1SortedListPool<T0, T1> where T0 : IPoolResettable where T1 : IPoolResettable, new()
{
    /// <summary>
    /// Retrieves an instance of SortedList from the pool.
    /// </summary>
    public static SortedList<T0, T1> Rent() => SortedListPool<T0, T1>.Rent();

    /// <summary>
    /// Resets each key and value in the SortedList by invoking <see cref="IPoolResettable.OnReturn"/>; the SortedList itself is not returned to the pool.
    /// </summary>
    /// <param name="value">SortedList whose entries to reset. Null values are ignored.</param>
    public static void Reset(SortedList<T0, T1> value)
    {
        if (value is null)
            return;

        foreach (KeyValuePair<T0, T1> entry in value)
        {
            entry.Key?.OnReturn();
            entry.Value?.OnReturn();
        }
    }

    /// <summary>
    /// Resets each entry in the SortedList via <see cref="Reset"/> and returns the SortedList to the pool.
    /// </summary>
    /// <param name="value">SortedList to return. Null values are ignored.</param>
    public static void Return(SortedList<T0, T1> value)
    {
        if (value is null)
            return;

        Reset(value);

        SortedListPool<T0, T1>.Return(value);
    }

    /// <summary>
    /// Calls <see cref="Return"/> on the SortedList and sets the original reference to null.
    /// </summary>
    /// <param name="value">SortedList to return; cleared to null after the call.</param>
    public static void ReturnAndNullifyReference(ref SortedList<T0, T1> value)
    {
        Return(value);

        value = null;
    }
}
