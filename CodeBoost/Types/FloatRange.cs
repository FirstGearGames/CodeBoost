using System;
using System.Runtime.CompilerServices;
using CodeBoost.Mathematics;

namespace CodeBoost.Types;

[Serializable]
public struct FloatRange
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FloatRange(float minimum, float maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary>
    /// The minimum value of the range.
    /// </summary>
    public float Minimum;
    /// <summary>
    /// The maximum value of the range.
    /// </summary>
    public float Maximum;

    /// <summary>
    /// Returns a random value between Minimum and Maximum.
    /// </summary>
    /// <returns> A random value greater than or equal to Minimum and less than or equal to Maximum. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float RandomInclusive() => (float)MathCb.RandomInclusive(Minimum, Maximum);

    /// <summary>
    /// Interpolates between Minimum and Maximum using a percentage.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Lerp(float percentage) => (float)MathCb.Lerp(Minimum, Maximum, percentage);
}