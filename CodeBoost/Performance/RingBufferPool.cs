using System.Collections.Generic;
using CodeBoost.Types;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for RingBuffer collections.
/// </summary>
public static class RingBufferPool<T0>
{
    /// <summary>
    /// Rents a RingBuffer from the pool.
    /// </summary>
    /// <returns>A cleared RingBuffer collection.</returns>
    public static RingBuffer<T0> Rent() => PoolStacks<RingBuffer<T0>>.Rent();

    /// <summary>
    /// Returns a RingBuffer to the pool and sets the provided reference to null.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref RingBuffer<T0> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Returns a RingBuffer to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(RingBuffer<T0> value)
    {
        if (value is null)
            return;

        value.Clear();

        PoolStacks<RingBuffer<T0>>.Return(value);
    }
}
