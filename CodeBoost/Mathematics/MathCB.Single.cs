using CodeBoost.Extensions;

namespace CodeBoost.Mathematics;

/// <summary>
/// Contains various utility methods relating to floating point numbers.
/// </summary>
public static partial class MathCb
{
    /// <summary>
    /// Returns true if all values are within tolerance of each other.
    /// </summary>
    /// <remarks>True is returned if values are null or empty.</remarks>
    public static bool AreValuesMatching(float[] values)
    {
        //Null array, return as matching since there is nothing to compare.
        if (values is null)
            return true;

        int length = values.Length;

        //No values, must match.
        if (length <= 1)
            return true;

        //Cache first value for quicker referencing.
        float firstValue = values[0];

        for (int i = 1; i < length; i++)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (firstValue != values[i])
                return false;
        }

        // If this far all values match.
        return true;
    }

    /// <summary>
    /// Returns a clamped float within a specified range.
    /// </summary>
    /// <param name = "value"> Value to clamp. </param>
    /// <param name = "minimum"> Minimum value. </param>
    /// <param name = "maximum"> Maximum value. </param>
    /// <returns>The value clamped within the specified range.</returns>
    public static float Clamp(float value, float minimum, float maximum)
    {
        if (value < minimum)
            value = minimum;
        else if (value > maximum)
            value = maximum;

        return value;
    }
        
        
    /// <summary>
    /// Converts a single to an Int32.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than 1f.</param>
    public static int SingleToInt32Unsafe(double value, float accuracy)
    {
        int wholeValue = (int)Clamp((int)(value * (1f / accuracy)), int.MinValue, int.MaxValue);
        return wholeValue;
    }

    /// <summary>
    /// Converts a single to a UInt32.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than 1f.</param>
    public static uint SingleToUInt32Unsafe(double value, float accuracy)
    {
        int wholeValue = (int)Clamp((int)(value * (1f / accuracy)), int.MinValue, int.MaxValue);

        return wholeValue.ToUInt32();
    }
}