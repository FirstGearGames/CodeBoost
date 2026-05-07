using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CodeBoost.Types;

[Serializable]
public struct Vector4Int
{
    /// <summary>
    /// The X value.
    /// </summary>
    public int X;
    /// <summary>
    /// The Y value.
    /// </summary>
    public int Y;
    /// <summary>
    /// The Z value.
    /// </summary>
    public int Z;
    /// <summary>
    /// The W value.
    /// </summary>
    public int W;

    /// <summary>
    /// Creates a new Vector4Int using the specified values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector4Int(int x = 0, int y = 0, int z = 0, int w = 0)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    /// <summary>
    /// Creates a new Vector4Int using the specified value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector4Int(Vector4Int vector4Int)
    {
        X = vector4Int.X;
        Y = vector4Int.Y;
        Z = vector4Int.Z;
        W = vector4Int.W;
    }

    /// <summary>
    /// Creates a new Vector4Int using the specified value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector4Int(Vector4 vector4, MidpointRounding rounding)
    {
        X = (int)Math.Round(vector4.X, rounding);
        Y = (int)Math.Round(vector4.Y, rounding);
        Z = (int)Math.Round(vector4.Z, rounding);
        W = (int)Math.Round(vector4.W, rounding);
    }
}