using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CodeBoost.Extensions;

public static class Vector3Extensions
{
    /// <summary>
    /// Adds the X and Y components of a Vector2 to the X and Y components of a Vector3.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector3 AddVector2(this System.Numerics.Vector3 a, System.Numerics.Vector2 b) => new(a.X + b.X, a.Y + b.Y, a.Z);

    /// <summary>
    /// Subtracts the X and Y components of a Vector2 from the X and Y components of a Vector3.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector3 SubtractVector2(this System.Numerics.Vector3 a, System.Numerics.Vector2 b) => new(a.X - b.X, a.Y - b.Y, a.Z);

    /// <summary>
    /// Returns true if the distance between the two values is equal to or less than the tolerance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinDistance(this System.Numerics.Vector3 a, System.Numerics.Vector3 b, float tolerance = 0.01f) => System.Numerics.Vector3.Distance(a, b) <= tolerance;

    /// <summary>
    /// Returns true if the squared distance between the two values is equal to or less than the tolerance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinDistanceSquared(this System.Numerics.Vector3 a, System.Numerics.Vector3 b, float tolerance = 0.01f) => System.Numerics.Vector3.DistanceSquared(a, b) <= tolerance;

    /// <summary>
    /// Returns true if any values within the Vector3 are NaN.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNan(this System.Numerics.Vector3 value)
    {
        return float.IsNaN(value.X) || float.IsNaN(value.Y) || float.IsNaN(value.Z);
    }

    /// <summary>
    /// Quantizes each component of the supplied <see cref="System.Numerics.Vector3"/> to the nearest multiple of the requested accuracy with clamping.
    /// </summary>
    /// <param name="value">Vector to quantize.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <param name="rounding">Rounding strategy applied during quantization.</param>
    /// <returns>The quantized vector with each component snapped to the requested accuracy.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector3 Quantize(this System.Numerics.Vector3 value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    {
        return new System.Numerics.Vector3(value.X.Quantize(accuracy, rounding), value.Y.Quantize(accuracy, rounding), value.Z.Quantize(accuracy, rounding));
    }

    /// <summary>
    /// Quantizes each component of the supplied <see cref="System.Numerics.Vector3"/> to the nearest multiple of the requested accuracy without range checking.
    /// </summary>
    /// <param name="value">Vector to quantize.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <param name="rounding">Rounding strategy applied during quantization.</param>
    /// <returns>The quantized vector, which may overflow if any scaled component falls outside the range of <see cref="int"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector3 QuantizeUnsafe(this System.Numerics.Vector3 value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    {
        return new System.Numerics.Vector3(value.X.QuantizeUnsafe(accuracy, rounding), value.Y.QuantizeUnsafe(accuracy, rounding), value.Z.QuantizeUnsafe(accuracy, rounding));
    }
}
