namespace CodeBoost.Extensions;

public static class Int32Extensions
{
    /// <summary>
    /// Converts an Int32 to a UInt32 using ZigZag encoding.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    public static uint ToUInt32(this int value) => (uint)((value << 1) ^ (value >> 31));
        
    /// <summary>
    /// Converts an <see cref="int"/> to a <see cref="float"/> using the requested accuracy.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <returns>The converted floating-point value.</returns>
    public static float ToSingle(this int value, float accuracy) => value * accuracy;

    /// <summary>
    /// Returns whether the whole flags value contains the specified part.
    /// </summary>
    public static bool FastContains(this int whole, int part) => (whole & part) == part;
}