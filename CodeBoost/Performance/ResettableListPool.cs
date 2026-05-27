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

    //xml resets and returns, and nullifies the references.
    public static void ReturnAndNullifyReference(ref List<T0> value)
    {
        Return(value);

        value = null;
    }
    	
    //xml resets and returns.
    public static void Return(List<T0> value)
    {
        if (value is null)
            return;

        Reset(value);

        ListPool<T0>.Return(value);
    }
	
    //xml resets only.
    public static void Reset(List<T0> value)
    {
        foreach (T0 entry in value)
            entry?.OnRent();
    }
}