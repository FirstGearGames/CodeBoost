using System;
using CodeBoost.Mathematics;

namespace CodeBoost.Types;

[Serializable]
public struct FloatRange
{
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
    public float RandomInclusive() => (float)MathCb.RandomInclusive(Minimum, Maximum);

    /// <summary>
    /// Interpolates between Minimum and Maximum using a percentage.
    /// </summary>
    public float Lerp(float percentage) => (float)MathCb.Lerp(Minimum, Maximum, percentage);
}