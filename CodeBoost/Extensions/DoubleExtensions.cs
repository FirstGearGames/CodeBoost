using System;

namespace CodeBoost.Extensions;

public static class DoubleExtensions
{
    /// <summary>
    /// Converts the supplied <see cref="double"/> value into a clamped <see cref="long"/> using the requested accuracy.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1d</c>.</param>
    /// <param name="rounding">Rounding strategy applied to the scaled value before conversion.</param>
    /// <returns>The converted value, clamped to the range of <see cref="long"/>.</returns>
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
    public static double QuantizeUnsafe(this double value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    {
        long wholeValue = value.ToInt64Unsafe(accuracy, rounding);
        return wholeValue.ToDouble(accuracy);
    }
}