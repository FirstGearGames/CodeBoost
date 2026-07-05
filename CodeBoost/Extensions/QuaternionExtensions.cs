using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CodeBoost.Extensions;

public static class QuaternionExtensions
{
    /// <summary>
    /// Returns the angle between the two rotations in degrees, treating a rotation and its negation as identical.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Angle(this Quaternion a, Quaternion b)
    {
        float dot = Math.Min(Math.Abs(Quaternion.Dot(a, b)), 1f);

        return 2f * (float)Math.Acos(dot) * (180f / (float)Math.PI);
    }

    /// <summary>
    /// Returns true if the angle between the two rotations is equal to or less than the tolerance in degrees.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinAngle(this Quaternion a, Quaternion b, float tolerance = 0.1f) => a.Angle(b) <= tolerance;
}
