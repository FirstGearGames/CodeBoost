using System;

namespace CodeBoost.Types;

/// <summary>
/// Indicates the feature is exposed for convenience but is primarily for internal use, and that usage may change without warning.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class InternalApiAttribute : Attribute
{
    public string Details { get; }
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