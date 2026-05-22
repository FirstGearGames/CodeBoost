using System.Runtime.CompilerServices;
using CodeBoost.Types;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for a RingBuffer which is resettable.
/// </summary>
public static class ResettableRingBufferPool<T0> where T0 : IPoolResettable, new()
{
    /// <summary>
    /// Retrieves an instance of RingBuffer from the pool.
    /// </summary>
    public static RingBuffer<T0> Rent() => RingBufferPool<T0>.Rent();

    /// <summary>
    /// Walks every live slot in the RingBuffer, invokes <see cref="IPoolResettable.OnReturn"/> on each item, defaults reference slots, and resets the write state; the RingBuffer itself is not returned to the pool.
    /// </summary>
    /// <param name="value">RingBuffer whose items to reset. Null values are ignored.</param>
    public static void Reset(RingBuffer<T0> value)
    {
        if (value is null)
            return;

        RingBufferWalkState<T0> state = value.GetWalkState();
        int count = state.Count;
        if (count > 0)
        {
            bool isReferenceOrContainsReferences = RuntimeHelpers.IsReferenceOrContainsReferences<T0>();
            T0[] collection = state.Collection;
            int capacity = state.Capacity;
            int real = state.BaseRealIndex;

            for (int i = 0; i < count; i++)
            {
                collection[real]?.OnReturn();

                if (isReferenceOrContainsReferences)
                    collection[real] = default!;

                real++;
                if (real >= capacity)
                    real = 0;
            }
        }

        value.ResetWriteState();
    }

    /// <summary>
    /// Walks the RingBuffer via <see cref="Reset"/> and returns the RingBuffer to the pool.
    /// </summary>
    /// <param name="value">RingBuffer to return. Null values are ignored.</param>
    public static void Return(RingBuffer<T0> value)
    {
        if (value is null)
            return;

        Reset(value);

        RingBufferPool<T0>.ReturnAlreadyCleared(value);
    }

    /// <summary>
    /// Calls <see cref="Return"/> on the RingBuffer and sets the original reference to null.
    /// </summary>
    /// <param name="value">RingBuffer to return; cleared to null after the call.</param>
    public static void ReturnAndNullifyReference(ref RingBuffer<T0> value)
    {
        Return(value);

        value = null;
    }
}
