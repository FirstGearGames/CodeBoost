using System;
using System.Drawing;
using System.Runtime.InteropServices;
using CodeBoost.Mathematics;

namespace CodeBoost.Types;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UnifiedColor
{
    /// <summary>
    /// R value for the color as a byte, 0-255.
    /// </summary>
    public byte R;
    /// <summary>
    /// G value for the color as a byte, 0-255.
    /// </summary>
    public byte G;
    /// <summary>
    /// B value for the color as a byte, 0-255.
    /// </summary>
    public byte B;
    /// <summary>
    /// A value for the color as a byte, 0-255.
    /// </summary>
    public byte A;
    /// <summary>
    /// R value for the color as a single, 0-1f.
    /// </summary>
    public float Rf => R / 255.0f;
    /// <summary>
    /// G value for the color as a single, 0-1f.
    /// </summary>
    public float Gf => G / 255.0f;
    /// <summary>
    /// B value for the color as a single, 0-1f.
    /// </summary>
    public float Bf => B / 255.0f;
    /// <summary>
    /// A value for the color as a single, 0-1f.
    /// </summary>
    public float Af => A / 255.0f;

    public UnifiedColor(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public UnifiedColor(Color color)
    {
        R = color.R;
        G = color.G;
        B = color.B;
        A = color.A;
    }

    public UnifiedColor(float r, float g, float b, float a = 1f)
    {
        R = (byte)MathCb.Clamp(r * 255, 0, 255);
        G = (byte)MathCb.Clamp(g * 255, 0, 255);
        B = (byte)MathCb.Clamp(b * 255, 0, 255);
        A = (byte)MathCb.Clamp(a * 255, 0, 255);
    }

    /// <summary>
    /// Returns a Color using the current values.
    /// </summary>
    /// <returns></returns>
    public Color GetColor() => Color.FromArgb(A, R, G, B);
}