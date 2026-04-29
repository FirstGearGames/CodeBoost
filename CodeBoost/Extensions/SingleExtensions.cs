using System;
using CodeBoost.Mathematics;

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
    public static int ToInt32(this float value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    {
        long wholeValue = (long)Math.Round(value * (1f / accuracy), rounding);
        return (int)MathCb.Clamp(wholeValue, int.MinValue, int.MaxValue);
    }

    /// <summary>
    /// Converts the supplied <see cref="float"/> value into an <see cref="int"/> using the requested accuracy without range checking.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <param name="rounding">Rounding strategy applied to the scaled value before conversion.</param>
    /// <returns>The converted value, which may overflow if the scaled result falls outside the range of <see cref="int"/>.</returns>
    public static int ToInt32Unsafe(this float value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    {
        return (int)Math.Round(value * (1f / accuracy), rounding);
    }

    /// <summary>
    /// Quantizes the supplied <see cref="float"/> value to the nearest multiple of the requested accuracy with clamping.
    /// </summary>
    /// <param name="value">Value to quantize.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <param name="rounding">Rounding strategy applied during quantization.</param>
    /// <returns>The quantized value snapped to the requested accuracy.</returns>
    public static float Quantize(this float value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    {
        int wholeValue = value.ToInt32(accuracy, rounding);
        return wholeValue.ToSingle(accuracy);
    }

    /// <summary>
    /// Quantizes the supplied <see cref="float"/> value to the nearest multiple of the requested accuracy without range checking.
    /// </summary>
    /// <param name="value">Value to quantize.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <param name="rounding">Rounding strategy applied during quantization.</param>
    /// <returns>The quantized value snapped to the requested accuracy, which may overflow if the scaled result falls outside the range of <see cref="int"/>.</returns>
    public static float QuantizeUnsafe(this float value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    {
        int wholeValue = value.ToInt32Unsafe(accuracy, rounding);
        return wholeValue.ToSingle(accuracy);
    }
}