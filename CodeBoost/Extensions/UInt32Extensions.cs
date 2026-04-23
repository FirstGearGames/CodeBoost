using CodeBoost.Logging;

namespace CodeBoost.Extensions;

public static class UInt32Extensions
{
    /// <summary>
    /// Converts a UInt32 to an Int32 using ZigZag encoding.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    public static int ConvertToInt32(this uint value) => (int)((value >> 1) ^ (~(value & 1) + 1));
        
    /// <summary>
    /// Converts a UInt32 to a Single without error checking.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than 1f.</param>
    public static float ToSingleUnsafe(this uint value, float accuracy)
    {
        int signedValue = value.ConvertToInt32();
            
        float divisor = 1f / accuracy;
        float floatValue = signedValue / divisor;

        return floatValue;
    }
        
    /// <summary>
    /// Returns whether the whole flags value contains the specified part.
    /// </summary>
    public static bool FastContains(this uint whole, uint part) => (whole & part) == part;
        
}