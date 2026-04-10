using System;
using CodeBoost.Mathematics;

namespace CodeBoost.Types;

[Serializable]
public struct ByteRange
{
    public ByteRange(byte minimum, byte maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary>
    /// Minimum range.
    /// </summary>
    public byte Minimum;
    /// <summary>
    /// Maximum range.
    /// </summary>
    public byte Maximum;

    /// <summary>
    /// Returns an exclusive random value between Minimum and Maximum.
    /// </summary>
    /// <returns> </returns>
    public byte RandomExclusive() => (byte)MathCb.RandomExclusive(Minimum, Maximum);

    /// <summary>
    /// Returns an inclusive random value between Minimum and Maximum.
    /// </summary>
    /// <returns> </returns>
    public byte RandomInclusive() => (byte)MathCb.RandomInclusive(Minimum, Maximum);

    /// <summary>
    /// Clamps value between Minimum and Maximum.
    /// </summary>
    public byte Clamp(byte value) => (byte)MathCb.Clamp(value, Minimum, Maximum);

    /// <summary>
    /// True if value is within range of Minimum and Maximum.
    /// </summary>
    public bool InRange(byte value) => value >= Minimum && value <= Maximum;
}