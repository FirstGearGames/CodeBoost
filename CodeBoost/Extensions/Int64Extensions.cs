namespace CodeBoost.Extensions;

public static class Int64Extensions
{
    /// <summary>
    /// Converts an Int64 to a UInt64 using ZigZag encoding.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    public static ulong ConvertToUInt64(this long value) => (ulong)((value << 1) ^ (value >> 63));
        
    /// <summary>
    /// Converts an Int64 to a Double without error checking.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than 1f.</param>
    public static double ToDoubleUnsafe(this long value, float accuracy) 
    {
        float divisor = 1f / accuracy;
        float floatValue = value / divisor;

        return floatValue;
    }

    /// <summary>
    /// Returns if a flags whole value has part within it.
    /// </summary>
    public static bool FastContains(this long whole, long part) => (whole & part) == part;
}