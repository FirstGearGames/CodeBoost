using System;
using System.Collections;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for generic objects.
/// </summary>
public static class ObjectPool<T0> where T0 : new()
{
    static ObjectPool()
    {
        //A collection pooled here would come back without being cleared; its own pool clears it on return.
        if (typeof(IEnumerable).IsAssignableFrom(typeof(T0)))
            throw new InvalidOperationException($"[{typeof(T0).Name}] is a collection; use its collection pool, which clears it on return.");

        // if (typeof(IPoolResettable).IsAssignableFrom(typeof(T0)))
        // {
        //     Logger.LogError(typeof(ObjectPool<>), $"[{typeof(T0).Name}] implements IPoolResettable; use the Resettable pool instead.");
        //     return;
        // }
    }

    /// <summary>
    /// Rents a generic object from the pool.
    /// </summary>
    /// <returns>A new or pooled instance of T0.</returns>
    public static T0 Rent() => PoolStacks<T0>.Rent();

    /// <summary>
    /// Returns a generic object to the pool and sets the provided reference to null.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref T0 value)
    {
        Return(value);

        value = default!;
    }

    /// <summary>
    /// Returns a generic object to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(T0 value) => PoolStacks<T0>.Return(value);
}
