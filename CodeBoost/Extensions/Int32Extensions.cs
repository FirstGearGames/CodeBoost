using System;
using System.Runtime.CompilerServices;

namespace CodeBoost.Extensions;

public static class Int32Extensions
{
    /// <summary>
    /// Converts an Int32 to a UInt32 using ZigZag encoding.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToUInt32(this int value) => (uint)((value << 1) ^ (value >> 31));

    /// <summary>
    /// Converts an <see cref="int"/> to a <see cref="float"/> using the requested accuracy.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <returns>The converted floating-point value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToSingle(this int value, float accuracy) => (float)(value * (decimal)accuracy);

    /// <summary>
    /// Converts an <see cref="int"/> to a <see cref="float"/> using the requested accuracy without intermediate decimal precision.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <returns>The converted floating-point value.</returns>
    /// <remarks>Faster than <see cref="ToSingle"/> but may accumulate small rounding error for large magnitudes.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToSingleUnsafe(this int value, float accuracy) => value * accuracy;

    /// <summary>
    /// Divides the supplied value by the divisor and rounds the result.
    /// </summary>
    /// <param name="value">Value to divide.</param>
    /// <param name="divisor">Divisor to divide the value by.</param>
    /// <param name="rounding">Rounding strategy applied to the division result.</param>
    /// <returns>The rounded quotient of the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Divide(this int value, int divisor, MidpointRounding rounding = MidpointRounding.ToEven)
    {
        double result = (double)value / divisor;

        return (int)Math.Round(result, rounding);
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
    public static bool AreValuesMatching(this int[] values)
    {
        if (values is null)
            return true;

        if (values.Length == 0)
            return true;

        int firstValue = values[0];

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
    public static bool FastContains(this int whole, int part) => (whole & part) == part;
}
