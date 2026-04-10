using System.Text;

namespace CodeBoost.Extensions;

/// <summary>
/// Various utility classes relating to Float.
/// </summary>
public static class UInt16Extensions
{
    /// <summary>
    /// Returns if a flags whole value has part within it.
    /// </summary>
    public static bool FastContains(this ushort whole, ushort part) => (whole & part) == part;
}