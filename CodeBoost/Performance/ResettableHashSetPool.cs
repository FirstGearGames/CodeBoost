using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for a HashSet which is resettable.
/// </summary>
public static class ResettableHashSetPool<T0> where T0 : IPoolResettable, new()
{
    /// <summary>
    /// Retrieves an instance of HashSet from the pool.
    /// </summary>
    public static HashSet<T0> Rent() => HashSetPool<T0>.Rent();

    /// <summary>
    /// Resets each item in the HashSet by invoking <see cref="IPoolResettable.OnReturn"/>; the HashSet itself is not returned to the pool.
    /// </summary>
    /// <param name="value">HashSet whose items to reset. Null values are ignored.</param>
    public static void Reset(HashSet<T0> value)
    {
        if (value is null)
            return;

        foreach (T0 item in value)
            item?.OnReturn();
    }

    /// <summary>
    /// Resets each item in the HashSet via <see cref="Reset"/> and returns the HashSet to the pool.
    /// </summary>
    /// <param name="value">HashSet to return. Null values are ignored.</param>
    public static void Return(HashSet<T0> value)
    {
        if (value is null)
            return;

        Reset(value);

        HashSetPool<T0>.Return(value);
    }

    /// <summary>
    /// Calls <see cref="Return"/> on the HashSet and sets the original reference to null.
    /// </summary>
    /// <param name="value">HashSet to return; cleared to null after the call.</param>
    public static void ReturnAndNullifyReference(ref HashSet<T0> value)
    {
        Return(value);

        value = null;
    }
}
