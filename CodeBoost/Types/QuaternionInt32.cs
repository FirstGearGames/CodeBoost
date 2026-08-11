using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CodeBoost.Types;

[Serializable]
public struct QuaternionInt32
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
    /// Creates a new Vector3Int using the specified values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public QuaternionInt32(int x = 0, int y = 0, int z = 0, int w = 0)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

        
    /// <summary>
    /// Creates a new Vector3Int64 using the specified values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public QuaternionInt32(float x, float y, float z, float w, float accuracy)
    {
        MidpointRounding midpointRounding = MidpointRounding.ToEven;

        X = (int)Math.Round(x / accuracy, midpointRounding);
        Y = (int)Math.Round(y / accuracy, midpointRounding);
        Z = (int)Math.Round(z / accuracy, midpointRounding);
        W = (int)Math.Round(w / accuracy, midpointRounding);
    }

    /// <summary>
    /// Creates a new QuaternionInt32 using the specified value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public QuaternionInt32(QuaternionInt32 quaternionInt32)
    {
        X = quaternionInt32.X;
        Y = quaternionInt32.Y;
        Z = quaternionInt32.Z;
        W = quaternionInt32.W;
    }

    /// <summary>
    /// Creates a new QuaternionInt32 using the specified value and accuracy.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public QuaternionInt32(Quaternion quaternion, float accuracy)
    {
        MidpointRounding midpointRounding = MidpointRounding.ToEven;

        X = (int)Math.Round(quaternion.X / accuracy, midpointRounding);
        Y = (int)Math.Round(quaternion.Y / accuracy, midpointRounding);
        Z = (int)Math.Round(quaternion.Z / accuracy, midpointRounding);
        W = (int)Math.Round(quaternion.W / accuracy, midpointRounding);
    }
}