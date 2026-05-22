using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for a Dictionary whose keys and values are both resettable.
/// </summary>
public static class ResettableT0T1DictionaryPool<T0, T1> where T0 : IPoolResettable where T1 : IPoolResettable, new()
{
    /// <summary>
    /// Retrieves an instance of Dictionary from the pool.
    /// </summary>
    public static Dictionary<T0, T1> Rent() => DictionaryPool<T0, T1>.Rent();

    /// <summary>
    /// Resets each key and value in the Dictionary by invoking <see cref="IPoolResettable.OnReturn"/>; the Dictionary itself is not returned to the pool.
    /// </summary>
    /// <param name="value">Dictionary whose entries to reset. Null values are ignored.</param>
    public static void Reset(Dictionary<T0, T1> value)
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
    /// Resets each entry in the Dictionary via <see cref="Reset"/> and returns the Dictionary to the pool.
    /// </summary>
    /// <param name="value">Dictionary to return. Null values are ignored.</param>
    public static void Return(Dictionary<T0, T1> value)
    {
        if (value is null)
            return;

        Reset(value);

        DictionaryPool<T0, T1>.Return(value);
    }

    /// <summary>
    /// Calls <see cref="Return"/> on the Dictionary and sets the original reference to null.
    /// </summary>
    /// <param name="value">Dictionary to return; cleared to null after the call.</param>
    public static void ReturnAndNullifyReference(ref Dictionary<T0, T1> value)
    {
        Return(value);

        value = null;
    }
}
