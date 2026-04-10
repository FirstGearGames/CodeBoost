namespace CodeBoost.Types;

public interface IWeighted
{
    /// <summary>
    /// Weight of item.
    /// </summary>
    float GetWeight();

    /// <summary>
    /// Number of times the item may be returned.
    /// </summary>
    /// <remarks>This value is typically the count of item.</remarks>
    UIntRange GetQuantityRange();
}