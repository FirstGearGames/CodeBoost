using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for a SortedDictionary whose values are resettable.
/// </summary>
public static class ResettableT1SortedDictionaryPool<T0, T1> where T1 : IPoolResettable, new()
{
    /// <summary>
    /// Retrieves an instance of SortedDictionary from the pool.
    /// </summary>
    public static SortedDictionary<T0, T1> Rent() => SortedDictionaryPool<T0, T1>.Rent();

    /// <summary>
    /// Resets each value in the SortedDictionary by invoking <see cref="IPoolResettable.OnReturn"/>; the SortedDictionary itself is not returned to the pool.
    /// </summary>
    /// <param name="value">SortedDictionary whose values to reset. Null values are ignored.</param>
    public static void Reset(SortedDictionary<T0, T1> value)
    {
        if (value is null)
            return;

        foreach (T1 item in value.Values)
            item?.OnReturn();
    }

    /// <summary>
    /// Resets each value in the SortedDictionary via <see cref="Reset"/> and returns the SortedDictionary to the pool.
    /// </summary>
    /// <param name="value">SortedDictionary to return. Null values are ignored.</param>
    public static void Return(SortedDictionary<T0, T1> value)
    {
        if (value is null)
            return;

        Reset(value);

        SortedDictionaryPool<T0, T1>.Return(value);
    }

    /// <summary>
    /// Calls <see cref="Return"/> on the SortedDictionary and sets the original reference to null.
    /// </summary>
    /// <param name="value">SortedDictionary to return; cleared to null after the call.</param>
    public static void ReturnAndNullifyReference(ref SortedDictionary<T0, T1> value)
    {
        Return(value);

        value = null;
    }
}
