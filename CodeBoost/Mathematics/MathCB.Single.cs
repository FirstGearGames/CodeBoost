using CodeBoost.Extensions;

namespace CodeBoost.Mathematics;

/// <summary>
/// Contains various utility methods relating to floating point numbers.
/// </summary>
public static partial class MathCb
{
    /// <summary>
    /// Returns a clamped float within a specified range.
    /// </summary>
    /// <param name="value">Value to clamp.</param>
    /// <param name="minimum">Minimum value.</param>
    /// <param name="maximum">Maximum value.</param>
    /// <returns>The value clamped within the specified range.</returns>
    public static float Clamp(float value, float minimum, float maximum)
    {
        if (value < minimum)
            value = minimum;
        else if (value > maximum)
            value = maximum;

        return value;
    }

    /// <summary>
    /// Converts a single to a UInt32 using ZigZag encoding after clamping into the range of <see cref="int"/>.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <returns>The converted UInt32 value.</returns>
    public static uint SingleToUInt32Unsafe(double value, float accuracy)
    {
        int wholeValue = (int)Clamp((int)(value * (1f / accuracy)), int.MinValue, int.MaxValue);

        return wholeValue.ToUInt32();
    }
}