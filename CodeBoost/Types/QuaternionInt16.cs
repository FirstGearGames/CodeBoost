using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CodeBoost.Types;

[Serializable]
public struct QuaternionInt16
{
    /// <summary>
    /// The X value.
    /// </summary>
    public short X;
    /// <summary>
    /// The Y value.
    /// </summary>
    public short Y;
    /// <summary>
    /// The Z value.
    /// </summary>
    public short Z;
    /// <summary>
    /// The W value.
    /// </summary>
    public short W;

    /// <summary>
    /// Creates a new QuaternionInt16 using the specified values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public QuaternionInt16(short x = 0, short y = 0, short z = 0, short w = 0)
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
    public QuaternionInt16(float x, float y, float z, float w, float accuracy)
    {
        MidpointRounding midpointRounding = MidpointRounding.ToEven;

        X = (short)Math.Round(x / accuracy, midpointRounding);
        Y = (short)Math.Round(y / accuracy, midpointRounding);
        Z = (short)Math.Round(z / accuracy, midpointRounding);
        W = (short)Math.Round(w / accuracy, midpointRounding);
    }

    
    /// <summary>
    /// Creates a new QuaternionInt16 using the specified value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public QuaternionInt16(QuaternionInt16 quaternionInt16)
    {
        X = quaternionInt16.X;
        Y = quaternionInt16.Y;
        Z = quaternionInt16.Z;
        W = quaternionInt16.W;
    }

    /// <summary>
    /// Creates a new QuaternionInt16 using the specified value and accuracy.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public QuaternionInt16(Quaternion quaternion, float accuracy)
    {
        MidpointRounding midpointRounding = MidpointRounding.ToEven;
        
        X = (short)Math.Round(quaternion.X / accuracy, midpointRounding);
        Y = (short)Math.Round(quaternion.Y / accuracy, midpointRounding);
        Z = (short)Math.Round(quaternion.Z / accuracy, midpointRounding);
        W = (short)Math.Round(quaternion.W / accuracy, midpointRounding);
    }
}
