using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for a Dictionary which is resettable.
/// </summary>
public static class ResettableT0T1SortedListPool<T0, T1> where T0 : IPoolResettable, new() where T1 : IPoolResettable, new()
{
    /// <summary>
    /// Retrieves an instance of SortedList from the pool.
    /// </summary>
    public static SortedList<T0, T1> Rent() => SortedListPool<T0, T1>.Rent();

    /// <summary>
    /// Resets the SortedList, returns it to the pool, and nullifies the reference.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref SortedList<T0, T1> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Resets the SortedList and returns it to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(SortedList<T0, T1> value)
    {
        if (value is null)
            return;

        Reset(value);

        SortedListPool<T0, T1>.Return(value);
    }

    /// <summary>
    /// Resets the SortedList without returning it to the pool. Every contained key and value is returned through
    /// <see cref="ResettableObjectPool{T0}.Return"/>, so its <see cref="IPoolResettable.OnReturn"/> runs and the instance
    /// re-enters its pool rather than becoming garbage.
    /// </summary>
    /// <param name = "value"> Value to reset. </param>
    public static void Reset(SortedList<T0, T1> value)
    {
        /* Indexed rather than enumerated: SortedList's Keys and Values collections hand out reference-type enumerators per
         * foreach, while indexing the cached wrappers allocates nothing. */
        IList<T0> keys = value.Keys;
        IList<T1> values = value.Values;

        for (int index = 0; index < keys.Count; index++)
        {
            ResettableObjectPool<T0>.Return(keys[index]);
            ResettableObjectPool<T1>.Return(values[index]);
        }
    }
}