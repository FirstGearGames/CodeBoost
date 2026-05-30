using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using CodeBoost.Extensions;

namespace CodeBoost.Types;

[Serializable]
public struct FloatRange2D
{
    public FloatRange X;
    public FloatRange Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FloatRange2D(FloatRange x, FloatRange y)
    {
        X = x;
        Y = y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FloatRange2D(float xMin, float xMax, float yMin, float yMax)
    {
        X = new(xMin, xMax);
        Y = new(yMin, yMax);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Clamp(Vector2 original) => new(ClampX(original.X), ClampY(original.Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Clamp(Vector3 original) => new(ClampX(original.X), ClampY(original.Y), original.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ClampX(float original) => original.Clamp(X.Minimum, X.Maximum);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ClampY(float original) => original.Clamp(Y.Minimum, Y.Maximum);
}