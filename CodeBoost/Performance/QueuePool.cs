using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for Queue collections.
/// </summary>
public static class QueuePool<T0>
{
    /// <summary>
    /// Rents a Queue from the pool.
    /// </summary>
    /// <returns>A cleared Queue collection.</returns>
    public static Queue<T0> Rent() => PoolStacks<Queue<T0>>.Rent();

    /// <summary>
    /// Returns a Queue to the pool and sets the provided reference to null.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref Queue<T0> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Returns a Queue to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(Queue<T0> value)
    {
        if (value is null)
            return;

        value.Clear();

        PoolStacks<Queue<T0>>.Return(value);
    }
}
