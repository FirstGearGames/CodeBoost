namespace CodeBoost.Extensions;

/// <summary>
/// Various utility classes relating to Float.
/// </summary>
public static class Int8Extensions
{
    /// <summary>
    /// Returns if a flags whole value has part within it.
    /// </summary>
    public static bool FastContains(this sbyte whole, sbyte part) => (whole & part) == part;
}