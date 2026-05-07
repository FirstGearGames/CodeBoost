using System.Numerics;
using System.Runtime.CompilerServices;

namespace CodeBoost.Extensions;

public static class Vector2Extensions
{
    /// <summary>
    /// Adds the X and Y components of a Vector3 to the X and Y components of a Vector2.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 AddVector3(this Vector2 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y);

    /// <summary>
    /// Subtracts the X and Y components of a Vector2 from the X and Y components of another Vector2.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 SubtractVector2(this Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);

    /// <summary>
    /// Returns true if the distance between the two values is equal to or less than the tolerance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinDistance(this Vector2 a, Vector2 b, float tolerance = 0.01f) => Vector2.Distance(a, b) <= tolerance;

    /// <summary>
    /// Returns true if the squared distance between the two values is equal to or less than the tolerance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinSquaredDistance(this Vector2 a, Vector2 b, float tolerance = 0.01f) => Vector2.DistanceSquared(a, b) <= tolerance;

    /// <summary>
    /// Returns true if any values within the Vector2 are NaN.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNan(this Vector2 value)
    {
        return float.IsNaN(value.X) || float.IsNaN(value.Y);
    }

    /// <summary>
    /// Quantizes each component of the supplied <see cref="Vector2"/> to the nearest multiple of the requested accuracy with clamping.
    /// </summary>
    /// <param name="value">Vector to quantize.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <param name="rounding">Rounding strategy applied during quantization.</param>
    /// <returns>The quantized vector with each component snapped to the requested accuracy.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Quantize(this Vector2 value, float accuracy, System.MidpointRounding rounding = System.MidpointRounding.AwayFromZero)
    {
        return new(value.X.Quantize(accuracy, rounding), value.Y.Quantize(accuracy, rounding));
    }

    /// <summary>
    /// Quantizes each component of the supplied <see cref="Vector2"/> to the nearest multiple of the requested accuracy without range checking.
    /// </summary>
    /// <param name="value">Vector to quantize.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <param name="rounding">Rounding strategy applied during quantization.</param>
    /// <returns>The quantized vector, which may overflow if any scaled component falls outside the range of <see cref="int"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 QuantizeUnsafe(this Vector2 value, float accuracy, System.MidpointRounding rounding = System.MidpointRounding.AwayFromZero)
    {
        return new(value.X.QuantizeUnsafe(accuracy, rounding), value.Y.QuantizeUnsafe(accuracy, rounding));
    }
}
