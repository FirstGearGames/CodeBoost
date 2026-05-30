using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for a List which is resettable.
/// </summary>
public static class ResettableListPool<T0> where T0 : IPoolResettable, new()
{
    /// <summary>
    /// Retrieves an instance of List from the pool.
    /// </summary>
    public static List<T0> Rent() => ListPool<T0>.Rent();

    /// <summary>
    /// Resets the List, returns it to the pool, and nullifies the reference.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref List<T0> value)
    {
        Return(value);

        value = null;
    }
    	
    /// <summary>
    /// Resets the List and returns it to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(List<T0> value)
    {
        if (value is null)
            return;

        Reset(value);

        ListPool<T0>.Return(value);
    }
	
    /// <summary>
    /// Resets the List without returning it to the pool.
    /// </summary>
    /// <param name = "value"> Value to reset. </param>
    public static void Reset(List<T0> value)
    {
        foreach (T0 entry in value)
            entry?.OnRent();
    }
}