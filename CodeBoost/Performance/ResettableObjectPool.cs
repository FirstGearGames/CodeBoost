
namespace CodeBoost.Performance;

/// <summary>
/// A pool for generic objects which are resettable.
/// </summary>
/// <remarks>This rents from the shared stacks directly rather than through <see cref="ObjectPool{T0}"/>, so a resettable collection, which clears itself in <see cref="IPoolResettable.OnReturn"/>, is not refused as an uncleared collection.</remarks>
public static class ResettableObjectPool<T0> where T0 : IPoolResettable, new()
{
    /// <summary>
    /// Retrieves an instance of T0 from the pool.
    /// </summary>
    public static T0 Rent()
    {
        T0 result = PoolStacks<T0>.Rent();
            
        result.OnRent();
            
        return result;
    }

    /// <summary>
    /// Stores an instance of T0 and sets the original reference to default.
    /// The method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref T0 value)
    {
        Return(value);
            
        value = default!;
    }

    /// <summary>
    /// Stores an instance of T0 in the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(T0 value)
    {
        if (value is null)
            return;

        value.OnReturn();
            
        PoolStacks<T0>.Return(value);
    }
}