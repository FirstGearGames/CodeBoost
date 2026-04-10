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
    /// Minimum range.
    /// </summary>
    public int Minimum;
    /// <summary>
    /// Maximum range.
    /// </summary>
    public int Maximum;

    /// <summary>
    /// Returns an exclusive random value between Minimum and Maximum.
    /// </summary>
    /// <returns> </returns>
    public int RandomExclusive() => (int)MathCb.RandomExclusive(Minimum, Maximum);

    /// <summary>
    /// Returns an inclusive random value between Minimum and Maximum.
    /// </summary>
    /// <returns> </returns>
    public int RandomInclusive() => (int)MathCb.RandomInclusive(Minimum, Maximum);

    /// <summary>
    /// Clamps a value between minimum and maximum.
    /// </summary>
    /// <returns>Clamped value.</returns>
    public int Clamp(int value) => (int)MathCb.Clamp(value, Minimum, Maximum);
}