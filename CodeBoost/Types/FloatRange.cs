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
    /// Minimum range.
    /// </summary>
    public float Minimum;
    /// <summary>
    /// Maximum range.
    /// </summary>
    public float Maximum;

    /// <summary>
    /// Returns a random value between Minimum and Maximum.
    /// </summary>
    /// <returns> </returns>
    public float RandomInclusive() => (float)MathCb.RandomInclusive(Minimum, Maximum);

    /// <summary>
    /// Interpolates between minimum and maximum using a percentage.
    /// </summary>
    public float Lerp(float percentage) => (float)MathCb.Lerp(Minimum, Maximum, percentage);
}