namespace CodeBoost.Extensions;

/// <summary>
/// Various utility classes relating to Float.
/// </summary>
public static class Int8Extensions
{
    /// <summary>
    /// Returns whether the whole flags value contains the specified part.
    /// </summary>
    public static bool FastContains(this sbyte whole, sbyte part) => (whole & part) == part;
}