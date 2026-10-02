using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for Dictionary collections.
/// </summary>
public static class DictionaryPool<T0, T1>
{
    /// <summary>
    /// Rents a Dictionary from the pool.
    /// </summary>
    /// <returns>A cleared Dictionary collection.</returns>
    public static Dictionary<T0, T1> Rent() => PoolStacks<Dictionary<T0, T1>>.Rent();

    /// <summary>
    /// Returns a Dictionary to the pool and sets the provided reference to null.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref Dictionary<T0, T1> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Returns a Dictionary to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(Dictionary<T0, T1> value)
    {
        if (value is null)
            return;

        value.Clear();

        PoolStacks<Dictionary<T0, T1>>.Return(value);
    }
}
