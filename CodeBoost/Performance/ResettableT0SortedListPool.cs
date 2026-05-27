using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for a Dictionary which is resettable.
/// </summary>
public static class ResettableT0SortedListPool<T0, T1> where T0 : IPoolResettable, new()
{
    /// <summary>
    /// Retrieves an instance of SortedList from the pool.
    /// </summary>
    public static SortedList<T0, T1> Rent() => SortedListPool<T0, T1>.Rent();

    //xml resets and returns, and nullifies the references.
    public static void ReturnAndNullifyReference(ref SortedList<T0, T1> value)
    {
        Return(value);

        value = null;
    }

    //xml resets and returns.
    public static void Return(SortedList<T0, T1> value)
    {
        if (value is null)
            return;
        
        SortedListPool<T0, T1>.Return(value);
    }

    //xml resets only.
    public static void Reset(SortedList<T0, T1> value)
    {
        foreach (T0 entry in value.Keys)
            entry?.OnReturn();
    }
}