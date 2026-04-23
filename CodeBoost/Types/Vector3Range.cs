using System;
using CodeBoost.Mathematics;
using SystemVector3 = System.Numerics.Vector3;

namespace CodeBoost.Types;

[Serializable]
public struct Vector3Range
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
    /// The Z range.
    /// </summary>
    public FloatRange Z;

    /// <summary>
    /// Creates ranges using the minimum and maximum values for each axis.
    /// </summary>
    public Vector3Range(SystemVector3 minimum, SystemVector3 maximum)
    {
        X = new(minimum.X, maximum.X);
        Y = new(minimum.Y, maximum.Y);
        Z = new(minimum.Z, maximum.Z);
    }

    /// <summary>
    /// Creates ranges using the specified minimum and maximum values.
    /// </summary>
    public Vector3Range(float minimum, float maximum)
    {
        X = new(minimum, maximum);
        Y = new(minimum, maximum);
        Z = new(minimum, maximum);
    }

    /// <summary>
    /// Returns a random value between Minimum and Maximum.
    /// </summary>
    /// <returns> A random Vector3 with each component in the inclusive range of its axis. </returns>
    public SystemVector3 RandomInclusive()
    {
        float x = (float)MathCb.RandomInclusive(X.Minimum, X.Maximum);
        float y = (float)MathCb.RandomInclusive(Y.Minimum, Y.Maximum);
        float z = (float)MathCb.RandomInclusive(Z.Minimum, Z.Maximum);

        return new(x, y, z);
    }
}