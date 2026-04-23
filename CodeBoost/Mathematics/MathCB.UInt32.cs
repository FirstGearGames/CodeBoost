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
    public static uint Divide(this uint value, uint divisor, RoundingType roundingType) 
    {
        if (divisor == 0)
        {
            Logger.LogError(typeof(MathCb), $"The divisor cannot be 0.");
            return 0;
        }
            
        switch (roundingType) 
        {
            case RoundingType.ToEven:
                return (uint)Math.Round((float)value / divisor, MidpointRounding.ToEven);
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
    /// Determines if all values passed in are the same.
    /// </summary>
    /// <param name = "values"> Values to check. </param>
    /// <returns> True if all values are the same. </returns>
    public static bool AreValuesMatching(uint[] values)
    {
        if (values is null)
            return true;

        if (values.Length == 0)
            return true;

        // Assign first value as element in first array.
        uint firstValue = values[0];
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