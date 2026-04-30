using System;
using System.Runtime.CompilerServices;

namespace CodeBoost.Extensions;

public static class Int64Extensions
{
    /// <summary>
    /// Converts an Int64 to a UInt64 using ZigZag encoding.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToUInt64(this long value) => (ulong)((value << 1) ^ (value >> 63));

    /// <summary>
    /// Converts a <see cref="long"/> to a <see cref="double"/> using the requested accuracy.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <returns>The converted double-precision value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDouble(this long value, float accuracy) => value * accuracy;

    /// <summary>
    /// Divides the supplied value by the divisor and rounds the result.
    /// </summary>
    /// <param name="value">Value to divide.</param>
    /// <param name="divisor">Divisor to divide the value by.</param>
    /// <param name="rounding">Rounding strategy applied to the division result.</param>
    /// <returns>The rounded quotient of the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Divide(this long value, long divisor, MidpointRounding rounding = MidpointRounding.ToEven)
    {
        double result = (double)value / divisor;

        return (long)Math.Round(result, rounding);
    }

    /// <summary>
    /// Returns the supplied value formatted with leading zeros up to the requested padding width.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <param name="padding">Total minimum width of the resulting string.</param>
    /// <returns>The padded string representation of the value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Pad(this long value, int padding)
    {
        if (padding < 0)
            padding = 0;

        return value.ToString().PadLeft(padding, '0');
    }

    /// <summary>
    /// Returns whether every value in the supplied array is equal to the first value.
    /// </summary>
    /// <remarks>
    /// True is returned when the array is null or empty.
    /// </remarks>
    /// <param name="values">Values to compare.</param>
    /// <returns>True when every value matches the first.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreValuesMatching(this long[] values)
    {
        if (values is null)
            return true;

        if (values.Length == 0)
            return true;

        long firstValue = values[0];

        for (int i = 1; i < values.Length; i++)
        {
            if (firstValue != values[i])
                return false;
        }

        return true;
    }

    /// <summary>
    /// Returns whether the whole flags value contains the specified part.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FastContains(this long whole, long part) => (whole & part) == part;
}
