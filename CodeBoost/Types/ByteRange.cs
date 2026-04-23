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
    public byte RandomExclusive() => (byte)MathCb.RandomExclusive(Minimum, Maximum);

    /// <summary>
    /// Returns an inclusive random value between Minimum and Maximum.
    /// </summary>
    /// <returns> A random value greater than or equal to Minimum and less than or equal to Maximum. </returns>
    public byte RandomInclusive() => (byte)MathCb.RandomInclusive(Minimum, Maximum);

    /// <summary>
    /// Clamps the value between Minimum and Maximum.
    /// </summary>
    public byte Clamp(byte value) => (byte)MathCb.Clamp(value, Minimum, Maximum);

    /// <summary>
    /// Returns true if the value is within the range of Minimum and Maximum.
    /// </summary>
    public bool InRange(byte value) => value >= Minimum && value <= Maximum;
}