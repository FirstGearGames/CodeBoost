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
    public static Vector3 AddVector2(this Vector3 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y, a.Z);

    /// <summary>
    /// Subtracts the X and Y components of a Vector2 from the X and Y components of a Vector3.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 SubtractVector2(this Vector3 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y, a.Z);

    /// <summary>
    /// Returns true if the distance between the two values is equal to or less than the tolerance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinDistance(this Vector3 a, Vector3 b, float tolerance = 0.01f) => Vector3.Distance(a, b) <= tolerance;

    /// <summary>
    /// Returns true if the squared distance between the two values is equal to or less than the tolerance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinDistanceSquared(this Vector3 a, Vector3 b, float tolerance = 0.01f) => Vector3.DistanceSquared(a, b) <= tolerance;

    /// <summary>
    /// Returns true if any values within the Vector3 are NaN.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNan(this Vector3 value)
    {
        return float.IsNaN(value.X) || float.IsNaN(value.Y) || float.IsNaN(value.Z);
    }

    // /// <summary>
    // /// Quantizes each component of the supplied <see cref="System.Numerics.Vector3"/> to the nearest multiple of the requested accuracy with clamping.
    // /// </summary>
    // /// <param name="value">Vector to quantize.</param>
    // /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    // /// <param name="rounding">Rounding strategy applied during quantization.</param>
    // /// <returns>The quantized vector with each component snapped to the requested accuracy.</returns>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static Vector3 Quantize(this Vector3 value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    // {
    //     return new(value.X.Quantize(accuracy, rounding), value.Y.Quantize(accuracy, rounding), value.Z.Quantize(accuracy, rounding));
    // }

    // /// <summary>
    // /// Quantizes each component of the supplied <see cref="System.Numerics.Vector3"/> to the nearest multiple of the requested accuracy without range checking.
    // /// </summary>
    // /// <param name="value">Vector to quantize.</param>
    // /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    // /// <param name="rounding">Rounding strategy applied during quantization.</param>
    // /// <returns>The quantized vector, which may overflow if any scaled component falls outside the range of <see cref="int"/>.</returns>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static Vector3 QuantizeUnsafe(this Vector3 value, float accuracy, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    // {
    //     return new(value.X.QuantizeUnsafe(accuracy, rounding), value.Y.QuantizeUnsafe(accuracy, rounding), value.Z.QuantizeUnsafe(accuracy, rounding));
    // }

    /// <summary>
    /// Returns true when every component of <paramref name="value"/> is within <paramref name="accuracy"/> of the corresponding component of <paramref name="otherValue"/>.
    /// </summary>
    /// <param name="value">First vector.</param>
    /// <param name="otherValue">Second vector.</param>
    /// <param name="accuracy">Per-component accuracy tolerance.</param>
    /// <returns>True when every component lies within the accuracy tolerance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinAccuracy(this Vector3 value, Vector3 otherValue, float accuracy)
    {
        Vector3 difference = value - otherValue;
        return Math.Abs(difference.X) <= accuracy && Math.Abs(difference.Y) <= accuracy && Math.Abs(difference.Z) <= accuracy;
    }
}
