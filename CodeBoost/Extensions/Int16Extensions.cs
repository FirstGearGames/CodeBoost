using System.Runtime.CompilerServices;

namespace CodeBoost.Extensions;

/// <summary>
/// Various utility classes relating to Float.
/// </summary>
public static class Int16Extensions
{
    /// <summary>
    /// Returns whether the whole flags value contains the specified part.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FastContains(this short whole, short part) => (whole & part) == part;
}
