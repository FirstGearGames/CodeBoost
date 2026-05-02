using System;
using System.Runtime.CompilerServices;

namespace CodeBoost.Extensions;

/// <summary>
/// Extension methods for converting <see cref="float"/> values into other numeric forms.
/// </summary>
public static class SingleExtensions
{
    /// <summary>
    /// Converts the supplied <see cref="float"/> value into a clamped <see cref="int"/> using the requested accuracy.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <param name="rounding">Rounding strategy applied to the scaled value before conversion.</param>
    /// <returns>The converted value, clamped to the range of <see cref="int"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32(this float value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    {
        long wholeValue = (long)Math.Round(value * (1f / accuracy), rounding);
        return (int)Math.Clamp(wholeValue, int.MinValue, int.MaxValue);
    }

    /// <summary>
    /// Converts the supplied <see cref="float"/> value into an <see cref="int"/> using the requested accuracy without range checking.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <param name="rounding">Rounding strategy applied to the scaled value before conversion.</param>
    /// <returns>The converted value, which may overflow if the scaled result falls outside the range of <see cref="int"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32Unsafe(this float value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    {
        return (int)Math.Round(value * (1f / accuracy), rounding);
    }

    // /// <summary>
    // /// Quantizes the supplied <see cref="float"/> value to the nearest multiple of the requested accuracy with clamping.
    // /// </summary>
    // /// <param name="value">Value to quantize.</param>
    // /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    // /// <param name="rounding">Rounding strategy applied during quantization.</param>
    // /// <returns>The quantized value snapped to the requested accuracy.</returns>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static float Quantize(this float value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    // {
    //     int wholeValue = value.ToInt32(accuracy, rounding);
    //     return wholeValue.ToSingle(accuracy);
    // }

    // /// <summary>
    // /// Quantizes the supplied <see cref="float"/> value to the nearest multiple of the requested accuracy without range checking.
    // /// </summary>
    // /// <param name="value">Value to quantize.</param>
    // /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    // /// <param name="rounding">Rounding strategy applied during quantization.</param>
    // /// <returns>The quantized value snapped to the requested accuracy, which may overflow if the scaled result falls outside the range of <see cref="int"/>.</returns>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static float QuantizeUnsafe(this float value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    // {
    //     int wholeValue = value.ToInt32Unsafe(accuracy, rounding);
    //     return wholeValue.ToSingle(accuracy);
    // }

    /// <summary>
    /// Returns whether every value in the supplied array matches the first value within the requested accuracy.
    /// </summary>
    /// <remarks>
    /// True is returned when the array is null or contains a single value.
    /// </remarks>
    /// <param name="values">Values to compare.</param>
    /// <param name="accuracy">Maximum allowed absolute difference between the first value and each subsequent value. A value is considered a match when its absolute difference from the first value is less than or equal to this tolerance.</param>
    /// <returns>True when every value lies within the accuracy tolerance of the first value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreValuesMatching(this float[] values, float accuracy = 0.0000001f)
    {
        if (values is null)
            return true;

        int length = values.Length;

        if (length <= 1)
            return true;

        float firstValue = values[0];
        for (int i = 1; i < length; i++)
        {
            float absDifference = Math.Abs(firstValue - values[i]);

            if (absDifference > accuracy)
                return false;
        }

        return true;
    }
}