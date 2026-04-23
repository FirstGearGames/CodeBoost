using System.Globalization;
using CodeBoost.Types;

namespace CodeBoost.Mathematics;

/// <summary>
/// Contains various utility methods relating to floating point numbers.
/// </summary>
public static partial class MathCb
{
    /// <summary>
    /// Pads the whole value of a double by a max number of indexes.
    /// </summary>
    /// <returns>The padded string representation of the value.</returns>
    public static string PadLeft(double value, int padding)
    {
        return value.ToString(CultureInfo.InvariantCulture).PadLeft(padding, '0');
    }

    /// <summary>
    /// Returns a random value between a minimum and an inclusive maximum.
    /// </summary>
    /// <returns>A random value between the minimum and the inclusive maximum.</returns>
    /// <remarks>The maximum value is padded with double.Epsilon to achieve the closest possibility to inclusive.</remarks>
    public static double RandomInclusive(double minimum, double maximum)
    {
        double range = maximum - minimum + double.Epsilon;
        double multiplier = Random.NextDouble() * range;
        return multiplier + minimum;
    }

    /// <summary>
    /// Returns a random value between a minimum and an exclusive maximum.
    /// </summary>
    /// <returns>A random value between the minimum and the exclusive maximum.</returns>
    public static double RandomExclusive(double minimum, double maximum)
    {
        double range = maximum - minimum;
        double multiplier = Random.NextDouble() * range;
        return multiplier + minimum;
    }

    /// <summary>
    /// Clamps a value between 0 and 1.
    /// </summary>
    public static double Clamp01(double value)
    {
        if (value < 0d)
            return 0d;

        if (value > 1d)
            return 1d;

        return value;
    }

    /// <summary>
    /// Returns a random value between 0f and 1f.
    /// </summary>
    /// <returns>A random value between 0f and 1f.</returns>
    public static double Random01() => RandomInclusive(0f, 1f);

    /// <summary>
    /// Interpolates between start and end using a percentage.
    /// </summary>
    public static double Lerp(double start, double end, double percentage)
    {
        percentage = Clamp01(percentage);

        return start + (end - start) * percentage;
    }

    /// <summary>
    /// Returns whether two values are within tolerance of each other.
    /// </summary>
    public static bool IsApproximately(this double a, double b, double tolerance = 0.00001d) => System.Math.Abs(a - b) <= tolerance;

    /// <summary>
    /// Returns the sign of a value as -1 or 1.
    /// </summary>
    /// >
    /// <remarks>A value of 0 will return 1.</remarks>
    public static double NonZeroSign(double value) => value >= 0 ? 1f : -1f;

    /// <summary>
    /// Returns true if a value is within the inclusive range of minimum and maximum.
    /// </summary>
    public static bool IsBetweenInclusive(double value, double minimum, double maximum) => value >= minimum && value <= maximum;

    /// <summary>
    /// Randomly inverts a value between positive and negative sign.
    /// </summary>
    public static double RandomSign(this float value)
    {
        if (RandomInclusive(0UL, 1UL) == 0)
            return value;

        return value * -1d;
    }

    /// <summary>
    /// Returns true if all values are within tolerance of each other.
    /// </summary>
    /// <remarks>True is returned if values are null or empty.</remarks>
    public static bool AreValuesMatching(double[] values)
    {
        //Null array, return as matching since there is nothing to compare.
        if (values is null)
            return true;

        int length = values.Length;

        //No values, must match.
        if (length <= 1)
            return true;

        //Cache first value for quicker referencing.
        double firstValue = values[0];

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
    /// Converts a double to an Int64.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than 1f.</param>
    public static long DoubleToInt64Unsafe(double value, float accuracy) 
    {
        long wholeValue = Clamp((long)(value * (1f / accuracy)), long.MinValue, long.MaxValue);
        return wholeValue;
    }
}