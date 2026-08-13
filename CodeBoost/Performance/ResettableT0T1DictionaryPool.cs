using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for a Dictionary which is resettable.
/// </summary>
public static class ResettableT0T1DictionaryPool<T0, T1> where T0 : IPoolResettable, new() where T1 : IPoolResettable, new()
{
    /// <summary>
    /// Retrieves an instance of Dictionary from the pool.
    /// </summary>
    public static Dictionary<T0, T1> Rent() => DictionaryPool<T0, T1>.Rent();

    /// <summary>
    /// Resets the Dictionary, returns it to the pool, and nullifies the reference.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref Dictionary<T0, T1> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Resets the Dictionary and returns it to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(Dictionary<T0, T1> value)
    {
        if (value is null)
            return;

        Reset(value);

        DictionaryPool<T0, T1>.Return(value);
    }
    
    /// <summary>
    /// Resets the Dictionary without returning it to the pool. Every contained key and value is returned through
    /// <see cref="ResettableObjectPool{T0}.Return"/>, so its <see cref="IPoolResettable.OnReturn"/> runs and the instance
    /// re-enters its pool rather than becoming garbage.
    /// </summary>
    /// <param name = "value"> Value to reset. </param>
    public static void Reset(Dictionary<T0, T1> value)
    {
        foreach (KeyValuePair<T0, T1> entry in value)
        {
            ResettableObjectPool<T0>.Return(entry.Key);
            ResettableObjectPool<T1>.Return(entry.Value);
        }
    }
}