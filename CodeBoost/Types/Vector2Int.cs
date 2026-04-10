using System;
using System.Numerics;

namespace CodeBoost.Types;

[Serializable]
public struct Vector2Int
{
    /// <summary>
    /// X value.
    /// </summary>
    public int X;
    /// <summary>
    /// Y value.
    /// </summary>
    public int Y;

    /// <summary>
    /// Creates a new Vector2Int using values.
    /// </summary>
    public Vector2Int(int x = 0, int y = 0)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Creates a new Vector2Int using value.
    /// </summary>
    public Vector2Int(Vector2Int vector2Int)
    {
        X = vector2Int.X;
        Y = vector2Int.Y;
    }

    /// <summary>
    /// Creates a new Vector2Int using value.
    /// </summary>
    public Vector2Int(Vector2 vector2, MidpointRounding rounding)
    {
        X = (int)Math.Round(vector2.X, rounding);
        Y = (int)Math.Round(vector2.Y, rounding);
    }
}