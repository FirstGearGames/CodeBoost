using System;
using System.Runtime.CompilerServices;
using CodeBoost.Logging;
using CodeBoost.Mathematics;

namespace CodeBoost.Extensions;

public static class UInt64Extensions
{
    /// <summary>
    /// The number of bits to use for calculating rounding up, such as to bytes or aligning to bytes.
    /// </summary>
    private const int RoundUpToByteBitCount = 7;

    /// <summary>
    /// Returns how many packed bytes the specified number of bits requires.
    /// </summary>
    /// <returns>The number of bytes required to pack bitCount.</returns>
    /// <remarks>When a 0 <see cref="bitCount"/> is provided the returned value will also be 0.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToPackedByteCount(this ulong bitCount) => (uint)(bitCount + 7) / 8;

    /// <summary>
    /// Converts a UInt64 to an Int64 using ZigZag encoding.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ConvertToInt64(this ulong value) => (long)((value >> 1) ^ (~(value & 1) + 1));

    /// <summary>
    /// Divides the supplied value by the divisor using the requested rounding strategy.
    /// </summary>
    /// <param name="value">Value to divide.</param>
    /// <param name="divisor">Divisor to divide the value by.</param>
    /// <param name="roundingType">Rounding type to use.</param>
    /// <returns>The rounded quotient of the division, or <c>0</c> when the divisor is <c>0</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Divide(this ulong value, ulong divisor, RoundingType roundingType)
    {
        if (divisor == 0)
        {
            Logger.LogError(typeof(UInt64Extensions), $"The divisor cannot be 0.");
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
                Logger.LogError(typeof(UInt64Extensions), $"The rounding type [{roundingType}] is unhandled.");
                return 0;
        }
    }

    /// <summary>
    /// Returns the supplied value formatted with leading zeros up to the requested padding width.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <param name="padding">Total minimum width of the resulting string.</param>
    /// <returns>The padded string representation of the value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Pad(this ulong value, int padding)
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
    public static bool AreValuesMatching(this ulong[] values)
    {
        if (values is null)
            return true;

        if (values.Length == 0)
            return true;

        ulong firstValue = values[0];

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
    public static bool FastContains(this ulong whole, ulong part) => (whole & part) == part;
}
