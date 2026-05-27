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

    //xml resets and returns, and nullifies the references.
    public static void ReturnAndNullifyReference(ref BoostedQueue<T0> value)
    {
        Return(value);

        value = null;
    }
    
    //xml resets and returns.
    public static void Return(BoostedQueue<T0> value)
    {
        if (value is null)
            return;

        Reset(value);
        
        BoostedQueuePool<T0>.Return(value);
    }

    //xml resets only.
    public static void Reset(BoostedQueue<T0> value) 
    {
        bool isReferenceOrContainsReferences = RuntimeHelpers.IsReferenceOrContainsReferences<T0>();

        while (value.TryDequeue(out T0 entry, defaultArrayEntry: isReferenceOrContainsReferences))
            entry?.OnReturn();
    }
}