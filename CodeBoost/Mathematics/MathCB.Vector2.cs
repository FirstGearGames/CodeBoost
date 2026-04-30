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
    public static float InverseLerp(Vector2 a, Vector2 b, Vector2 value)
    {
        Vector2 ab = b - a;
        Vector2 av = value - a;

        float dotA = Vector2.Dot(av, ab);
        float dotB = Vector2.Dot(ab, ab);

        return (float)((double)(dotA / dotB)).Clamp01();
    }

    /// <summary>
    /// Interpolates between three Vector2 values.
    /// </summary>
    /// <returns>The interpolated Vector2 value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Lerp3(Vector2 a, Vector2 b, Vector2 c, float percentage)
    {
        Vector2 r0 = Vector2.Lerp(a, b, percentage);
        Vector2 r1 = Vector2.Lerp(b, c, percentage);
        return Vector2.Lerp(r0, r1, percentage);
    }
}
