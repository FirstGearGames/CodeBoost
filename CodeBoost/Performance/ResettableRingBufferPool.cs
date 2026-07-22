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
    /// Resets the RingBuffer, returns it to the pool, and nullifies the reference.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref RingBuffer<T0> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Stores an instance of RingBuffer in the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(RingBuffer<T0> value)
    {
        if (value is null)
            return;

        Reset(value);

        RingBufferPool<T0>.Return(value);
    }

    /// <summary>
    /// Resets the RingBuffer without returning it to the pool. Every populated entry is returned through
    /// <see cref="ResettableObjectPool{T0}.Return"/> — so its <see cref="IPoolResettable.OnReturn"/> runs and the instance
    /// re-enters its pool rather than becoming garbage — then the collection is cleared so no reset entries remain reachable.
    /// </summary>
    /// <param name = "value"> Value to reset. </param>
    public static void Reset(RingBuffer<T0> value)
    {
        foreach (T0 entry in value)
            ResettableObjectPool<T0>.Return(entry);

        value.Clear();
    }
}