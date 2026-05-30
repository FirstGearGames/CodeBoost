using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeBoost.Extensions;

namespace CodeBoost.Types;

/// <summary>
/// Represents a color stored as RGBA byte components, with helper accessors for floating-point values.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UnifiedColor
{
    /// <summary>
    /// The R value for the color as a byte, 0-255.
    /// </summary>
    public byte R;
    /// <summary>
    /// The G value for the color as a byte, 0-255.
    /// </summary>
    public byte G;
    /// <summary>
    /// The B value for the color as a byte, 0-255.
    /// </summary>
    public byte B;
    /// <summary>
    /// The A value for the color as a byte, 0-255.
    /// </summary>
    public byte A;
    /// <summary>
    /// The R value for the color as a single, 0-1f.
    /// </summary>
    public float Rf
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => R / 255.0f;
    }
    /// <summary>
    /// The G value for the color as a single, 0-1f.
    /// </summary>
    public float Gf
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => G / 255.0f;
    }
    /// <summary>
    /// The B value for the color as a single, 0-1f.
    /// </summary>
    public float Bf
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => B / 255.0f;
    }
    /// <summary>
    /// The A value for the color as a single, 0-1f.
    /// </summary>
    public float Af
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => A / 255.0f;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiedColor"/> struct from byte components.
    /// </summary>
    /// <param name="r">Red component, in the range 0-255.</param>
    /// <param name="g">Green component, in the range 0-255.</param>
    /// <param name="b">Blue component, in the range 0-255.</param>
    /// <param name="a">Alpha component, in the range 0-255.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UnifiedColor(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiedColor"/> struct by copying the components of a <see cref="Color"/>.
    /// </summary>
    /// <param name="color">Color to copy components from.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UnifiedColor(Color color)
    {
        R = color.R;
        G = color.G;
        B = color.B;
        A = color.A;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiedColor"/> struct from floating-point components in the range 0-1.
    /// </summary>
    /// <param name="r">Red component, in the range 0-1.</param>
    /// <param name="g">Green component, in the range 0-1.</param>
    /// <param name="b">Blue component, in the range 0-1.</param>
    /// <param name="a">Alpha component, in the range 0-1.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UnifiedColor(float r, float g, float b, float a = 1f)
    {
        R = (byte)(r * 255).Clamp(0, 255);
        G = (byte)(g * 255).Clamp(0, 255);
        B = (byte)(b * 255).Clamp(0, 255);
        A = (byte)(a * 255).Clamp(0, 255);
    }

    /// <summary>
    /// Returns a <see cref="Color"/> constructed from the current ARGB component values.
    /// </summary>
    /// <returns>A <see cref="Color"/> constructed from the current A, R, G, and B values.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Color GetColor() => Color.FromArgb(A, R, G, B);
}