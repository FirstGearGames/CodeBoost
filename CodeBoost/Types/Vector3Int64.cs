using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CodeBoost.Types;

[Serializable]
public struct Vector3Int64
{
    /// <summary>
    /// The X value.
    /// </summary>
    public long X;
    /// <summary>
    /// The Y value.
    /// </summary>
    public long Y;
    /// <summary>
    /// The Z value.
    /// </summary>
    public long Z;

    /// <summary>
    /// Creates a new Vector3Int64 using the specified values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3Int64(long x = 0, long y = 0, long z = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }
    
    /// <summary>
    /// Creates a new Vector3Int using the specified values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3Int64(double x, double y, double z, float accuracy)
    {
        MidpointRounding midpointRounding = MidpointRounding.ToEven;

        X = (long)Math.Round(x / accuracy, midpointRounding);
        Y = (long)Math.Round(y / accuracy, midpointRounding);
        Z = (long)Math.Round(z / accuracy, midpointRounding);
    }

    /// <summary>
    /// Creates a new Vector3Int using the specified value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3Int64(Vector3Int32 vector3Int32)
    {
        X = vector3Int32.X;
        Y = vector3Int32.Y;
        Z = vector3Int32.Z;
    }

    /// <summary>
    /// Creates a new Vector3Int64 using the specified value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3Int64(Vector3Int64 vector3Int64)
    {
        X = vector3Int64.X;
        Y = vector3Int64.Y;
        Z = vector3Int64.Z;
    }

    /// <summary>
    /// Creates a new Vector3Int64 using the specified value and accuracy.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3Int64(Vector3 vector3, float accuracy)
    {
        MidpointRounding midpointRounding = MidpointRounding.ToEven;

        X = (long)Math.Round(vector3.X / accuracy, midpointRounding);
        Y = (long)Math.Round(vector3.Y / accuracy, midpointRounding);
        Z = (long)Math.Round(vector3.Z / accuracy, midpointRounding);
    }
}