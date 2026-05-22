using System.Runtime.CompilerServices;
using CodeBoost.Types;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for a BoostedQueue which is resettable.
/// </summary>
public static class ResettableBoostedQueuePool<T0> where T0 : IPoolResettable, new()
{
    /// <summary>
    /// Retrieves an instance of BoostedQueue from the pool.
    /// </summary>
    public static BoostedQueue<T0> Rent() => BoostedQueuePool<T0>.Rent();

    /// <summary>
    /// Drains the BoostedQueue, invoking <see cref="IPoolResettable.OnReturn"/> on each item, and resets the queue's write state; the BoostedQueue itself is not returned to the pool.
    /// </summary>
    /// <param name="value">BoostedQueue whose items to reset. Null values are ignored.</param>
    public static void Reset(BoostedQueue<T0> value)
    {
        if (value is null)
            return;

        bool isReferenceOrContainsReferences = RuntimeHelpers.IsReferenceOrContainsReferences<T0>();

        while (value.TryDequeue(out T0 item, defaultArrayEntry: isReferenceOrContainsReferences))
            item?.OnReturn();

        value.ResetWriteState();
    }

    /// <summary>
    /// Drains the BoostedQueue via <see cref="Reset"/> and returns the BoostedQueue to the pool.
    /// </summary>
    /// <param name="value">BoostedQueue to return. Null values are ignored.</param>
    public static void Return(BoostedQueue<T0> value)
    {
        if (value is null)
            return;

        Reset(value);

        BoostedQueuePool<T0>.ReturnAlreadyCleared(value);
    }

    /// <summary>
    /// Calls <see cref="Return"/> on the BoostedQueue and sets the original reference to null.
    /// </summary>
    /// <param name="value">BoostedQueue to return; cleared to null after the call.</param>
    public static void ReturnAndNullifyReference(ref BoostedQueue<T0> value)
    {
        Return(value);

        value = null;
    }
}
