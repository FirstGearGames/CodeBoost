using System.Globalization;
using CodeBoost.Types;

namespace CodeBoost.Mathematics;

/// <summary>
/// Contains various utility methods relating to floating point numbers.
/// </summary>
public static partial class MathCb
{
    /// <summary>
    /// Returns the supplied value formatted with leading zeros up to the requested padding width.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <param name="padding">Total minimum width of the resulting string.</param>
    /// <returns>The padded string representation of the value.</returns>
    public static string PadLeft(double value, int padding)
    {
        return value.ToString(CultureInfo.InvariantCulture).PadLeft(padding, '0');
    }

    /// <summary>
    /// Returns a random value between the supplied minimum and an inclusive maximum.
    /// </summary>
    /// <remarks>
    /// The maximum value is padded with <see cref="double.Epsilon"/> to achieve the closest possibility to inclusive.
    /// </remarks>
    /// <param name="minimum">Inclusive minimum value.</param>
    /// <param name="maximum">Inclusive maximum value.</param>
    /// <returns>A random value between the minimum and the inclusive maximum.</returns>
    public static double RandomInclusive(double minimum, double maximum)
    {
        double range = maximum - minimum + double.Epsilon;
        double multiplier = Random.NextDouble() * range;
        return multiplier + minimum;
    }

    /// <summary>
    /// Returns a random value between the supplied minimum and an exclusive maximum.
    /// </summary>
    /// <param name="minimum">Inclusive minimum value.</param>
    /// <param name="maximum">Exclusive maximum value.</param>
    /// <returns>A random value between the minimum and the exclusive maximum.</returns>
    public static double RandomExclusive(double minimum, double maximum)
    {
        double range = maximum - minimum;
        double multiplier = Random.NextDouble() * range;
        return multiplier + minimum;
    }

    /// <summary>
    /// Clamps the supplied value into the inclusive range from <c>0</c> to <c>1</c>.
    /// </summary>
    /// <param name="value">Value to clamp.</param>
    /// <returns>The clamped value.</returns>
    public static double Clamp01(double value)
    {
        if (value < 0d)
            return 0d;

        if (value > 1d)
            return 1d;

        return value;
    }

    /// <summary>
    /// Returns a random value between <c>0f</c> and <c>1f</c> inclusive.
    /// </summary>
    /// <returns>A random value between <c>0f</c> and <c>1f</c>.</returns>
    public static double Random01() => RandomInclusive(0f, 1f);

    /// <summary>
    /// Interpolates between <paramref name="start"/> and <paramref name="end"/> using the supplied percentage.
    /// </summary>
    /// <param name="start">Starting value of the interpolation.</param>
    /// <param name="end">Ending value of the interpolation.</param>
    /// <param name="percentage">Interpolation percentage in the range <c>0</c> to <c>1</c>.</param>
    /// <returns>The interpolated value.</returns>
    public static double Lerp(double start, double end, double percentage)
    {
        percentage = Clamp01(percentage);

        return start + (end - start) * percentage;
    }

    /// <summary>
    /// Returns whether the supplied values are within the requested tolerance of each other.
    /// </summary>
    /// <param name="a">First value to compare.</param>
    /// <param name="b">Second value to compare.</param>
    /// <param name="tolerance">Maximum allowed absolute difference.</param>
    /// <returns>True when the values are within the supplied tolerance.</returns>
    public static bool IsApproximately(this double a, double b, double tolerance = 0.00001d) => System.Math.Abs(a - b) <= tolerance;

    /// <summary>
    /// Returns the sign of the supplied value as <c>-1</c> or <c>1</c>.
    /// </summary>
    /// <remarks>
    /// A value of zero returns <c>1</c>.
    /// </remarks>
    /// <param name="value">Value whose sign is being inspected.</param>
    /// <returns>The sign of the value as <c>-1</c> or <c>1</c>.</returns>
    public static double NonZeroSign(double value) => value >= 0 ? 1f : -1f;

    /// <summary>
    /// Returns whether the supplied value lies within the inclusive range from <paramref name="minimum"/> to <paramref name="maximum"/>.
    /// </summary>
    /// <param name="value">Value to inspect.</param>
    /// <param name="minimum">Inclusive minimum value.</param>
    /// <param name="maximum">Inclusive maximum value.</param>
    /// <returns>True when the value lies within the inclusive range.</returns>
    public static bool IsBetweenInclusive(double value, double minimum, double maximum) => value >= minimum && value <= maximum;

    /// <summary>
    /// Randomly inverts the sign of the supplied value.
    /// </summary>
    /// <param name="value">Value whose sign may be flipped.</param>
    /// <returns>The original value, or its negation, chosen at random.</returns>
    public static double RandomSign(this float value)
    {
        if (RandomInclusive(0UL, 1UL) == 0)
            return value;

        return value * -1d;
    }

    /// <summary>
    /// Returns whether every value in the supplied array is equal to the first value.
    /// </summary>
    /// <remarks>
    /// True is returned when the array is null or empty.
    /// </remarks>
    /// <param name="values">Values to compare.</param>
    /// <returns>True when every value matches the first.</returns>
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
    /// Converts the supplied <see cref="double"/> value into a clamped <see cref="long"/> using the requested accuracy.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <returns>The converted long value, clamped to the range of <see cref="long"/>.</returns>
    public static long DoubleToInt64Unsafe(double value, float accuracy)
    {
        long wholeValue = Clamp((long)(value * (1f / accuracy)), long.MinValue, long.MaxValue);
        return wholeValue;
    }
}