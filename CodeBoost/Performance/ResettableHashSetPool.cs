using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CodeBoost.Types;

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
    /// Resets the HashSet, returns it to the pool, and nullifies the reference.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref HashSet<T0> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Resets the HashSet and returns it to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(HashSet<T0> value)
    {
        if (value is null)
            return;

        Reset(value);

        HashSetPool<T0>.Return(value);
    }

    /// <summary>
    /// Resets the HashSet without returning it to the pool.
    /// </summary>
    /// <param name = "value"> Value to reset. </param>
    public static void Reset(HashSet<T0> value)
    {
        foreach (T0 entry in value)
            entry?.OnRent();
    }
}