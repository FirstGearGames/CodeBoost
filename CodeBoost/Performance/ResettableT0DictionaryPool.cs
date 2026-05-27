using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for a Dictionary which is resettable.
/// </summary>
public static class ResettableT0DictionaryPool<T0, T1> where T0 : IPoolResettable, new()
{
    /// <summary>
    /// Retrieves an instance of Dictionary from the pool.
    /// </summary>
    public static Dictionary<T0, T1> Rent() => DictionaryPool<T0, T1>.Rent();

    //xml resets and returns, and nullifies the references.
    public static void ReturnAndNullifyReference(ref Dictionary<T0, T1> value)
    {
        Return(value);

        value = null;
    }

    //xml resets and returns.
    public static void Return(Dictionary<T0, T1> value)
    {
        if (value is null)
            return;

        Reset(value);

        DictionaryPool<T0, T1>.Return(value);
    }

    //xml resets only.
    public static void Reset(Dictionary<T0, T1> value)
    {
        foreach (T0 entry in value.Keys)
            entry?.OnReturn();
    }
}