using System;
using System.Runtime.CompilerServices;
using CodeBoost.Mathematics;

namespace CodeBoost.Types;

[Serializable]
public struct IntRange
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int RandomExclusive() => (int)MathCb.RandomExclusive(Minimum, Maximum);

    /// <summary>
    /// Returns an inclusive random value between Minimum and Maximum.
    /// </summary>
    /// <returns> A random value greater than or equal to Minimum and less than or equal to Maximum. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int RandomInclusive() => (int)MathCb.RandomInclusive(Minimum, Maximum);
}