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
    /// Resets the BoostedQueue, returns it to the pool, and nullifies the reference.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref BoostedQueue<T0> value)
    {
        Return(value);

        value = null;
    }
    
    /// <summary>
    /// Resets the BoostedQueue and returns it to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(BoostedQueue<T0> value)
    {
        if (value is null)
            return;

        Reset(value);
        
        BoostedQueuePool<T0>.Return(value);
    }

    /// <summary>
    /// Resets the BoostedQueue without returning it to the pool. Every entry is drained through <see cref="IPoolResettable.OnReturn"/>, then the read and write indices are restored to their pristine state.
    /// </summary>
    /// <param name = "value"> Value to reset. </param>
    public static void Reset(BoostedQueue<T0> value)
    {
        bool isReferenceOrContainsReferences = Polyfill.IsReferenceOrContainsReferences<T0>();

        while (value.TryDequeue(out T0 entry, defaultArrayEntry: isReferenceOrContainsReferences))
            entry?.OnReturn();

        value.ResetWriteState();
    }
}