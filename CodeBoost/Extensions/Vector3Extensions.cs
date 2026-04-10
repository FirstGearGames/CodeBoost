using System.Numerics;

namespace CodeBoost.Extensions;

public static class Vector3Extensions
{
    /// <summary>
    /// Adds Vector2.X/Y to a Vector3.X/Y.
    /// </summary>
    public static System.Numerics.Vector3 AddVector2(this System.Numerics.Vector3 a, System.Numerics.Vector2 b) => new(a.X + b.X, a.Y + b.Y, a.Z);

    /// <summary>
    /// Subtracts Vector2.X/Y from a Vector3.X/Y.
    /// </summary>
    public static System.Numerics.Vector3 SubtractVector2(this System.Numerics.Vector3 a, System.Numerics.Vector2 b) => new(a.X - b.X, a.Y - b.Y, a.Z);

    /// <summary>
    /// True if the distance of two values are equal to or less than the tolerance.
    /// </summary>
    public static bool IsWithinDistance(this System.Numerics.Vector3 a, System.Numerics.Vector3 b, float tolerance = 0.01f) => System.Numerics.Vector3.Distance(a, b) <= tolerance;

    /// <summary>
    /// True if the distance of two values are equal to or less than the tolerance.
    /// </summary>
    public static bool IsWithinDistanceSquared(this System.Numerics.Vector3 a, System.Numerics.Vector3 b, float tolerance = 0.01f) => System.Numerics.Vector3.DistanceSquared(a, b) <= tolerance;

    /// <summary>
    /// True if any values within a Vector3 are NaN.
    /// </summary>
    public static bool IsNan(this System.Numerics.Vector3 value)
    {
        return float.IsNaN(value.X) || float.IsNaN(value.Y) || float.IsNaN(value.Z);
    }
}