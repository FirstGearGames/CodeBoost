using System;
using System.Numerics;
using CodeBoost.Mathematics;

namespace CodeBoost.Types;

[Serializable]
public struct Vector2Range
{
    /// <summary>
    /// The X range.
    /// </summary>
    public FloatRange X;
    /// <summary>
    /// The Y range.
    /// </summary>
    public FloatRange Y;

    /// <summary>
    /// Creates ranges using the minimum and maximum values for each axis.
    /// </summary>
    public Vector2Range(Vector2 minimum, Vector2 maximum)
    {
        X = new(minimum.X, maximum.X);
        Y = new(minimum.Y, maximum.Y);
    }

    /// <summary>
    /// Creates ranges using the specified minimum and maximum values.
    /// </summary>
    public Vector2Range(float minimum, float maximum)
    {
        X = new(minimum, maximum);
        Y = new(minimum, maximum);
    }

    /// <summary>
    /// Returns a random value between Minimum and Maximum.
    /// </summary>
    /// <returns> A random Vector2 with each component in the inclusive range of its axis. </returns>
    public Vector2 RandomInclusive()
    {
        float x = (float)MathCb.RandomInclusive(X.Minimum, X.Maximum);
        float y = (float)MathCb.RandomInclusive(Y.Minimum, Y.Maximum);

        return new(x, y);
    }
}