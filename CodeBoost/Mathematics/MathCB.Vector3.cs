using System.Numerics;
using System.Runtime.CompilerServices;
using CodeBoost.Extensions;

namespace CodeBoost.Mathematics;

public static partial class MathCb
{
    /// <summary>
    /// Returns the normalized position of a value between a and b.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 ab = b - a;
        Vector3 av = value - a;

        float dotA = Vector3.Dot(av, ab);
        float dotB = Vector3.Dot(ab, ab);

        return (float)((double)(dotA / dotB)).Clamp01();
    }

    /// <summary>
    /// Interpolates between three Vector3 values.
    /// </summary>
    /// <returns>The interpolated Vector3 value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Lerp3(Vector3 a, Vector3 b, Vector3 c, float percentage)
    {
        Vector3 r0 = Vector3.Lerp(a, b, percentage);
        Vector3 r1 = Vector3.Lerp(b, c, percentage);
        return Vector3.Lerp(r0, r1, percentage);
    }
}
