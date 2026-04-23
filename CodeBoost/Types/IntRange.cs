using System;
using CodeBoost.Mathematics;

namespace CodeBoost.Types;

[Serializable]
public struct IntRange
{
    public IntRange(int minimum, int maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary>
    /// The minimum value of the range.
    /// </summary>
    public int Minimum;
    /// <summary>
    /// The maximum value of the range.
    /// </summary>
    public int Maximum;

    /// <summary>
    /// Returns an exclusive random value between Minimum and Maximum.
    /// </summary>
    /// <returns> A random value greater than or equal to Minimum and less than Maximum. </returns>
    public int RandomExclusive() => (int)MathCb.RandomExclusive(Minimum, Maximum);

    /// <summary>
    /// Returns an inclusive random value between Minimum and Maximum.
    /// </summary>
    /// <returns> A random value greater than or equal to Minimum and less than or equal to Maximum. </returns>
    public int RandomInclusive() => (int)MathCb.RandomInclusive(Minimum, Maximum);

    /// <summary>
    /// Clamps a value between Minimum and Maximum.
    /// </summary>
    /// <returns> The clamped value. </returns>
    public int Clamp(int value) => (int)MathCb.Clamp(value, Minimum, Maximum);
}