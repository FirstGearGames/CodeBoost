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
    /// Resets each item in the List by invoking <see cref="IPoolResettable.OnReturn"/>; the List itself is not returned to the pool.
    /// </summary>
    /// <param name="value">List whose items to reset. Null values are ignored.</param>
    public static void Reset(List<T0> value)
    {
        if (value is null)
            return;

        foreach (T0 item in value)
            item?.OnReturn();
    }

    /// <summary>
    /// Resets each item in the List via <see cref="Reset"/> and returns the List to the pool.
    /// </summary>
    /// <param name="value">List to return. Null values are ignored.</param>
    public static void Return(List<T0> value)
    {
        if (value is null)
            return;

        Reset(value);

        ListPool<T0>.Return(value);
    }

    /// <summary>
    /// Calls <see cref="Return"/> on the List and sets the original reference to null.
    /// </summary>
    /// <param name="value">List to return; cleared to null after the call.</param>
    public static void ReturnAndNullifyReference(ref List<T0> value)
    {
        Return(value);

        value = null;
    }
}
