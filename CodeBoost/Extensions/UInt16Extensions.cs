using System.Text;

namespace CodeBoost.Extensions;

/// <summary>
/// Various utility classes relating to Float.
/// </summary>
public static class UInt16Extensions
{
    /// <summary>
    /// Returns whether the whole flags value contains the specified part.
    /// </summary>
    public static bool FastContains(this ushort whole, ushort part) => (whole & part) == part;
}