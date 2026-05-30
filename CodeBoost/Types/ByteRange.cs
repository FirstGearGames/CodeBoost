using System;
using System.Runtime.CompilerServices;
using CodeBoost.Extensions;
using CodeBoost.Mathematics;

namespace CodeBoost.Types;

[Serializable]
public struct ByteRange
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteRange(byte minimum, byte maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary>
    /// The minimum value of the range.
    /// </summary>
    public byte Minimum;
    /// <summary>
    /// The maximum value of the range.
    /// </summary>
    public byte Maximum;

    /// <summary>
    /// Returns an exclusive random value between Minimum and Maximum.
    /// </summary>
    /// <returns> A random value greater than or equal to Minimum and less than Maximum. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte RandomExclusive() => (byte)MathCb.RandomExclusive(Minimum, Maximum);

    /// <summary>
    /// Returns an inclusive random value between Minimum and Maximum.
    /// </summary>
    /// <returns> A random value greater than or equal to Minimum and less than or equal to Maximum. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte RandomInclusive() => (byte)MathCb.RandomInclusive(Minimum, Maximum);

    /// <summary>
    /// Clamps the value between Minimum and Maximum.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Clamp(byte value) => value.Clamp(Minimum, Maximum);

    /// <summary>
    /// Returns true if the value is within the range of Minimum and Maximum.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool InRange(byte value) => value >= Minimum && value <= Maximum;
}