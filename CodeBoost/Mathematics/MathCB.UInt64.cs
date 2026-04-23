using System;
using CodeBoost.Logging;

namespace CodeBoost.Mathematics;

/// <summary>
/// Contains various utility methods relating to floating point numbers.
/// </summary>
public static partial class MathCb
{
    /// <summary>
    /// Divides a value by another and rounds the result.
    /// </summary>
    /// <param name="value">The number which is divided.</param>
    /// <param name="divisor">The divisor.</param>
    /// <param name="roundingType">Rounding type to use.</param>
    public static ulong Divide(this ulong value, ulong divisor, RoundingType roundingType) 
    {
        if (divisor == 0)
        {
            Logger.LogError(typeof(MathCb), $"The divisor cannot be 0.");
            return 0;
        }
            
        switch (roundingType) 
        {
            case RoundingType.ToEven:
                return (ulong)Math.Round((double)value / divisor, MidpointRounding.ToEven);
            case RoundingType.AwayFromZero:
                return (value + divisor / 2) / divisor;
            case RoundingType.Down:
                return value / divisor;
            case RoundingType.Up:
                return value == 0 ? 0 : (value + divisor - 1) / divisor;
            case RoundingType.UpNonZero:
                return value == 0 ? 1 : (value + divisor - 1) / divisor;
            default:
                Logger.LogError(typeof(MathCb), $"The rounding type [{roundingType}] is unhandled.");
                return 0;
        }
    }
        
    /// <summary>
    /// Pads an index by a specified value. This is preferred over typical padding so that pad values used with skins can be easily found in the code.
    /// </summary>
    public static string Pad(this ulong value, int padding)
    {
        if (padding < 0)
            padding = 0;

        return value.ToString().PadLeft(padding, '0');
    }

    /// <summary>
    /// Returns a clamped ulong within a specified range.
    /// </summary>
    /// <param name = "value"> Value to clamp. </param>
    /// <param name = "minimum"> Minimum value. </param>
    /// <param name = "maximum"> Maximum value. </param>
    /// <returns>The value clamped within the specified range.</returns>
    public static ulong Clamp(ulong value, ulong minimum, ulong maximum)
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
    public static ulong Min(ulong a, ulong b) => a < b ? a : b;

    /// <summary>
    /// Determines if all values passed in are the same.
    /// </summary>
    /// <param name = "values"> Values to check. </param>
    /// <returns> True if all values are the same. </returns>
    public static bool AreValuesMatching(ulong[] values)
    {
        if (values is null)
            return true;

        if (values.Length == 0)
            return true;

        // Assign first value as element in first array.
        ulong firstValue = values[0];
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
}