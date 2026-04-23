using CodeBoost.CodeAnalysis;

namespace CodeBoost.Performance;

/// <summary>
/// Implement this interface to reset values when returning to a pool, and to initialize them when renting from a pool.
/// </summary>
public interface IPoolResettable
{
    /// <summary>
    /// Resets the values when the instance is being placed into a pool.
    /// </summary>
    [CreateSignature]
    void OnReturn();

    /// <summary>
    /// Initializes the values when the instance is being retrieved from a pool.
    /// </summary>
    [CreateSignature]
    void OnRent();
}