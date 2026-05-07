using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CodeBoost.Types;

[Serializable]
public struct Vector2Int
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
    /// Creates a new Vector2Int using the specified values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2Int(int x = 0, int y = 0)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Creates a new Vector2Int using the specified value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2Int(Vector2Int vector2Int)
    {
        X = vector2Int.X;
        Y = vector2Int.Y;
    }

    /// <summary>
    /// Creates a new Vector2Int using the specified value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2Int(Vector2 vector2, MidpointRounding rounding)
    {
        X = (int)Math.Round(vector2.X, rounding);
        Y = (int)Math.Round(vector2.Y, rounding);
    }
}