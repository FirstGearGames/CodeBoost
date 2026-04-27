using System;

namespace CodeBoost.Types;

/// <summary>
/// Indicates the feature is exposed for convenience but is primarily for internal use, and that usage may change without warning.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class InternalApiAttribute : Attribute
{
    /// <summary>
    /// Optional details describing why the marked feature is considered internal.
    /// </summary>
    public string Details { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="InternalApiAttribute"/> class with optional details.
    /// </summary>
    /// <param name="details">Optional details describing why the marked feature is considered internal.</param>
    public InternalApiAttribute(string details = "") => Details = details;
}
    
/// <summary>
/// Indicates an object is only used by the server.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class ServerOnlyAttribute : Attribute
{
}
    
/// <summary>
/// Indicates an object is only used by the client.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class ClientOnlyAttribute : Attribute
{
}