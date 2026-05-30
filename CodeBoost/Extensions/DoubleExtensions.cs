using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace CodeBoost.Extensions;

public static class DoubleExtensions
{
    /// <summary>
    /// Returns the supplied value formatted with leading zeros up to the requested padding width.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <param name="padding">Total minimum width of the resulting string.</param>
    /// <returns>The padded string representation of the value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Pad(this double value, int padding)
    {
        return value.ToString(CultureInfo.InvariantCulture).PadLeft(padding, '0');
    }

    /// <summary>
    /// Clamps the supplied value into the inclusive range from <paramref name="minimum"/> to <paramref name="maximum"/>.
    /// </summary>
    /// <param name="value">Value to clamp.</param>
    /// <param name="minimum">Inclusive minimum value.</param>
    /// <param name="maximum">Inclusive maximum value.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Clamp(this double value, double minimum, double maximum)
    {
        if (value < minimum)
            return minimum;

        if (value > maximum)
            return maximum;

        return value;
    }

    /// <summary>
    /// Clamps the supplied value into the inclusive range from <c>0</c> to <c>1</c>.
    /// </summary>
    /// <param name="value">Value to clamp.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Clamp01(this double value)
    {
        if (value < 0d)
            return 0d;

        if (value > 1d)
            return 1d;

        return value;
    }

    /// <summary>
    /// Returns whether the supplied values are within the requested tolerance of each other.
    /// </summary>
    /// <param name="value">First value to compare.</param>
    /// <param name="other">Second value to compare.</param>
    /// <param name="tolerance">Maximum allowed absolute difference.</param>
    /// <returns>True when the values are within the supplied tolerance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsApproximately(this double value, double other, double tolerance = 0.00001d) => Math.Abs(value - other) <= tolerance;

    /// <summary>
    /// Returns the sign of the supplied value as <c>-1</c> or <c>1</c>.
    /// </summary>
    /// <remarks>
    /// A value of zero returns <c>1</c>.
    /// </remarks>
    /// <param name="value">Value whose sign is being inspected.</param>
    /// <returns>The sign of the value as <c>-1</c> or <c>1</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double NonZeroSign(this double value) => value >= 0 ? 1d : -1d;

    /// <summary>
    /// Returns whether the supplied value lies within the inclusive range from <paramref name="minimum"/> to <paramref name="maximum"/>.
    /// </summary>
    /// <param name="value">Value to inspect.</param>
    /// <param name="minimum">Inclusive minimum value.</param>
    /// <param name="maximum">Inclusive maximum value.</param>
    /// <returns>True when the value lies within the inclusive range.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenInclusive(this double value, double minimum, double maximum) => value >= minimum && value <= maximum;

    /// <summary>
    /// Returns whether every value in the supplied array is equal to the first value.
    /// </summary>
    /// <remarks>
    /// True is returned when the array is null or empty.
    /// </remarks>
    /// <param name="values">Values to compare.</param>
    /// <returns>True when every value matches the first.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreValuesMatching(this double[] values)
    {
        if (values is null)
            return true;

        int length = values.Length;

        if (length <= 1)
            return true;

        double firstValue = values[0];

        for (int i = 1; i < length; i++)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (firstValue != values[i])
                return false;
        }

        return true;
    }

    /// <summary>
    /// Converts the supplied <see cref="double"/> value into a clamped <see cref="long"/> using the requested accuracy.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1d</c>.</param>
    /// <param name="rounding">Rounding strategy applied to the scaled value before conversion.</param>
    /// <returns>The converted value, clamped to the range of <see cref="long"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToInt64(this double value, double accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    {
        double scaled = Math.Round(value * (1d / accuracy), rounding);

        if (scaled >= 9.223372036854775e18) return long.MaxValue;
        if (scaled <= -9.223372036854775e18) return long.MinValue;

        return (long)scaled;
    }

    /// <summary>
    /// Converts the supplied <see cref="double"/> value into a <see cref="long"/> using the requested accuracy without range checking.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1d</c>.</param>
    /// <param name="rounding">Rounding strategy applied to the scaled value before conversion.</param>
    /// <returns>The converted value, which may overflow if the scaled result falls outside the range of <see cref="long"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToInt64Unsafe(this double value, double accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    {
        return (long)Math.Round(value * (1d / accuracy), rounding);
    }

    /// <summary>
    /// Quantizes the supplied <see cref="double"/> value to the nearest multiple of the requested accuracy with clamping.
    /// </summary>
    /// <param name="value">Value to quantize.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <param name="rounding">Rounding strategy applied during quantization.</param>
    /// <returns>The quantized value snapped to the requested accuracy.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Quantize(this double value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    {
        long wholeValue = value.ToInt64(accuracy, rounding);
        return wholeValue.ToDouble(accuracy);
    }

    /// <summary>
    /// Quantizes the supplied <see cref="double"/> value to the nearest multiple of the requested accuracy without range checking.
    /// </summary>
    /// <param name="value">Value to quantize.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <param name="rounding">Rounding strategy applied during quantization.</param>
    /// <returns>The quantized value snapped to the requested accuracy, which may overflow if the scaled result falls outside the range of <see cref="long"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double QuantizeUnsafe(this double value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    {
        long wholeValue = value.ToInt64Unsafe(accuracy, rounding);
        return wholeValue.ToDouble(accuracy);
    }
}
