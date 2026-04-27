namespace CodeBoost.Types;

/// <summary>
/// Defines a weighted item that may be returned by a weighted-random selection.
/// </summary>
public interface IWeighted
{
    /// <summary>
    /// Returns the weight of the item.
    /// </summary>
    /// <returns>The weight of the item.</returns>
    float GetWeight();

    /// <summary>
    /// Returns the range of quantities that may be supplied for the item.
    /// </summary>
    /// <remarks>
    /// This value is typically the count of the item.
    /// </remarks>
    /// <returns>The quantity range for the item.</returns>
    UIntRange GetQuantityRange();
}