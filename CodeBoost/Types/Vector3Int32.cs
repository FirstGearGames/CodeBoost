using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CodeBoost.Types;

[Serializable]
public struct Vector3Int32
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
    /// Creates a new Vector3Int using the specified values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3Int32(int x = 0, int y = 0, int z = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Creates a new Vector3Int using the specified values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3Int32(float x, float y, float z, float accuracy)
    {
        MidpointRounding midpointRounding = MidpointRounding.ToEven;
            
        X = (int)Math.Round(x / accuracy, midpointRounding);
        Y = (int)Math.Round(y / accuracy, midpointRounding);
        Z = (int)Math.Round(z / accuracy, midpointRounding);
    }

    /// <summary>
    /// Creates a new Vector3Int using the specified value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3Int32(Vector3Int32 vector3Int32)
    {
        X = vector3Int32.X;
        Y = vector3Int32.Y;
        Z = vector3Int32.Z;
    }

    /// <summary>
    /// Creates a new Vector3Int using the specified value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3Int32(Vector3Int64 vector3Int64)
    {
        X = (int)vector3Int64.X;
        Y = (int)vector3Int64.Y;
        Z = (int)vector3Int64.Z;
    }

    /// <summary>
    /// Creates a new Vector3Int using the specified value and accuracy.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3Int32(Vector3 vector3, float accuracy)
    {
        MidpointRounding midpointRounding = MidpointRounding.ToEven;
        
        X = (int)Math.Round(vector3.X / accuracy, midpointRounding);
        Y = (int)Math.Round(vector3.Y / accuracy, midpointRounding);
        Z = (int)Math.Round(vector3.Z / accuracy, midpointRounding);
    }
}