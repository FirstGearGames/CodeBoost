namespace CodeBoost.Extensions;

public static class Int64Extensions
{
    /// <summary>
    /// Converts an Int64 to a UInt64 using ZigZag encoding.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    public static ulong ToUInt64(this long value) => (ulong)((value << 1) ^ (value >> 63));

    /// <summary>
    /// Converts a <see cref="long"/> to a <see cref="double"/> using the requested accuracy.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <returns>The converted double-precision value.</returns>
    public static double ToDouble(this long value, float accuracy) => value * accuracy;

    /// <summary>
    /// Returns whether the whole flags value contains the specified part.
    /// </summary>
    public static bool FastContains(this long whole, long part) => (whole & part) == part;
}