using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for a Queue which is resettable.
/// </summary>
public static class ResettableQueuePool<T0> where T0 : IPoolResettable, new()
{
    /// <summary>
    /// Retrieves an instance of Queue from the pool.
    /// </summary>
    public static Queue<T0> Rent() => QueuePool<T0>.Rent();

    /// <summary>
    /// Resets each item in the Queue by invoking <see cref="IPoolResettable.OnReturn"/>; the Queue itself is not returned to the pool.
    /// </summary>
    /// <param name="value">Queue whose items to reset. Null values are ignored.</param>
    public static void Reset(Queue<T0> value)
    {
        if (value is null)
            return;

        foreach (T0 item in value)
            item?.OnReturn();
    }

    /// <summary>
    /// Resets each item in the Queue via <see cref="Reset"/> and returns the Queue to the pool.
    /// </summary>
    /// <param name="value">Queue to return. Null values are ignored.</param>
    public static void Return(Queue<T0> value)
    {
        if (value is null)
            return;

        Reset(value);

        QueuePool<T0>.Return(value);
    }

    /// <summary>
    /// Calls <see cref="Return"/> on the Queue and sets the original reference to null.
    /// </summary>
    /// <param name="value">Queue to return; cleared to null after the call.</param>
    public static void ReturnAndNullifyReference(ref Queue<T0> value)
    {
        Return(value);

        value = null;
    }
}
