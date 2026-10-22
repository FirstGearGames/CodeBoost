using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for a SortedDictionary which is resettable.
/// </summary>
public static class ResettableT1SortedDictionaryPool<T0, T1> where T1 : IPoolResettable, new()
{
    /// <summary>
    /// Rents a SortedDictionary from the pool.
    /// </summary>
    /// <returns>A cleared SortedDictionary collection.</returns>
    public static SortedDictionary<T0, T1> Rent() => SortedDictionaryPool<T0, T1>.Rent();

    /// <summary>
    /// Resets the SortedDictionary, returns it to the pool, and nullifies the reference.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref SortedDictionary<T0, T1> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Resets the SortedDictionary and returns it to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(SortedDictionary<T0, T1> value)
    {
        if (value is null)
            return;

        Reset(value);

        SortedDictionaryPool<T0, T1>.Return(value);
    }

    /// <summary>
    /// Resets the SortedDictionary without returning it to the pool. Every contained value is returned through
    /// <see cref="ResettableObjectPool{T0}.Return"/>, so its <see cref="IPoolResettable.OnReturn"/> runs and the instance
    /// re-enters its pool rather than becoming garbage.
    /// The collection is then cleared, so it holds no entry its pool can hand to another renter.
    /// </summary>
    /// <param name = "value"> Value to reset. </param>
    public static void Reset(SortedDictionary<T0, T1> value)
    {
        foreach (T1 entry in value.Values)
            ResettableObjectPool<T1>.Return(entry);

        value.Clear();
    }
}
