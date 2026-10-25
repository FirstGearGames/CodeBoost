namespace System.Runtime.CompilerServices;

/// <summary>
/// Marks a type as an interpolated string handler, which the compiler builds in place of a string wherever an interpolated string is
/// passed to a parameter of that type.
/// </summary>
/// <remarks>Declared here because neither .NET Standard target CodeBoost builds for carries it. The compiler recognizes it by name, so this internal copy serves every consumer of the handlers declared with it.</remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
internal sealed class InterpolatedStringHandlerAttribute : Attribute
{
}
