namespace CodeBoost.Extensions;

public static class Int32Extensions
{
    /// <summary>
    /// Converts an Int32 to a UInt32 using ZigZag encoding.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    public static uint ConvertToUInt32(this int value) => (uint)((value << 1) ^ (value >> 31));
        
    /// <summary>
    /// Converts an Int32 to a Single.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than 1f.</param>
    public static float ToSingleUnsafe(this int value, float accuracy)
    {
        float divisor = 1f / accuracy;
        float floatValue = value / divisor;

        return floatValue;
    }

    /// <summary>
    /// Returns whether the whole flags value contains the specified part.
    /// </summary>
    public static bool FastContains(this int whole, int part) => (whole & part) == part;
}