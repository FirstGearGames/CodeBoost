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
    public static int Divide(int valueA, int valueB, MidpointRounding midpointRounding = MidpointRounding.ToEven) 
    {
        double result = (double)valueA / valueB;
            
        return (int)Math.Round(result, midpointRounding);
    }

    /// <summary>
    /// Determines if all values passed in are the same.
    /// </summary>
    /// <param name = "values"> Values to check. </param>
    /// <returns> True if all values are the same. </returns>
    public static bool AreValuesMatching(int[] values)
    {
        if (values is null)
            return true;

        if (values.Length == 0)
            return true;

        // Assign first value as element in first array.
        int firstValue = values[0];
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