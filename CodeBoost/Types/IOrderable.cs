namespace CodeBoost.Types;

/// <summary>
/// Defines a type that exposes an explicit ordering value used when sorting.
/// </summary>
public interface IOrderable
{
    /// <summary>
    /// The ordering value used when sorting instances.
    /// </summary>
    public int Order { get; }
}