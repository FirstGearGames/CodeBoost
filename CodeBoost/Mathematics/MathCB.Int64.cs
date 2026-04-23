using System;

namespace CodeBoost.Mathematics;

/// <summary>
/// Contains various utility methods relating to floating point numbers.
/// </summary>
public static partial class MathCb
{
    /// <summary>
    /// Divides a value by another and rounds the result.
    /// </summary>
    /// <param name="valueA">The number which is divided.</param>
    /// <param name="valueB">The divisor.</param>
    /// <param name="midpointRounding">Rounding type to use.</param>
    public static long Divide(long valueA, long valueB, MidpointRounding midpointRounding = MidpointRounding.ToEven) 
    {
        double result = (double)valueA / valueB;
            
        return (long)Math.Round(result, midpointRounding);
    }
        
    /// <summary>
    /// Pads an index by a specified value. This is preferred over typical padding so that pad values used with skins can be easily found in the code.
    /// </summary>
    public static string Pad(this long value, int padding)
    {
        if (padding < 0)
            padding = 0;

        return value.ToString().PadLeft(padding, '0');
    }

    /// <summary>
    /// Returns a clamped long within a specified range.
    /// </summary>
    /// <param name = "value"> Value to clamp. </param>
    /// <param name = "minimum"> Minimum value. </param>
    /// <param name = "maximum"> Maximum value. </param>
    /// <returns>The value clamped within the specified range.</returns>
    public static long Clamp(long value, long minimum, long maximum)
    {
        if (value < minimum)
            value = minimum;
        else if (value > maximum)
            value = maximum;

        return value;
    }

    /// <summary>
    /// Returns whichever value is lower.
    /// </summary>
    public static long Min(long a, long b) => a < b ? a : b;

    /// <summary>
    /// Determines if all values passed in are the same.
    /// </summary>
    /// <param name = "values"> Values to check. </param>
    /// <returns> True if all values are the same. </returns>
    public static bool AreValuesMatching(long[] values)
    {
        if (values is null)
            return true;

        if (values.Length == 0)
            return true;

        // Assign first value as element in first array.
        long firstValue = values[0];
        // Check all values.
        for (int i = 1; i < values.Length; i++)
        {
            // If any value doesn't match first value return false.
            if (firstValue != values[i])
                return false;
        }

        // If this far all values match.
        return true;
    }
        
    /// <summary>
    /// Converts an Int64 to a single.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than 1f.</param>
    public static double Int64ToSingleUnsafe(double value, float accuracy) 
    {
        float divisor = 1f / accuracy;
        double doubleValue = value / divisor;

        return doubleValue;
    }
}