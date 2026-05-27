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

    //xml resets and returns, and nullifies the references.
    public static void ReturnAndNullifyReference(ref Queue<T0> value)
    {
        Return(value);

        value = null;
    }

    //xml resets and returns.
    public static void Return(Queue<T0> value)
    {
        if (value is null)
            return;

        Reset(value);

        QueuePool<T0>.Return(value);
    }
    
    //xml resets only.
    public static void Reset(Queue<T0> value)
    {
        foreach (T0 entry in value)
            entry?.OnRent();
    }
}