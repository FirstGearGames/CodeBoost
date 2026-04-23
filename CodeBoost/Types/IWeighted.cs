namespace CodeBoost.Types;

public interface IWeighted
{
    /// <summary>
    /// The weight of the item.
    /// </summary>
    float GetWeight();

    /// <summary>
    /// The number of times the item may be returned.
    /// </summary>
    /// <remarks> This value is typically the count of the item. </remarks>
    UIntRange GetQuantityRange();
}