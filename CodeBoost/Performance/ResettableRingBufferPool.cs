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
    /// Stores an instance of RingBuffer and sets the original reference to null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref RingBuffer<T0> value, PoolReturnType collectionReturnType)
    {
        Return(value, collectionReturnType);

        value = null;
    }

    /// <summary>
    /// Stores an instance of RingBuffer in the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(RingBuffer<T0> value, PoolReturnType collectionReturnType)
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

        if (collectionReturnType is PoolReturnType.Return)
            RingBufferPool<T0>.ReturnAlreadyCleared(value);
    }
}