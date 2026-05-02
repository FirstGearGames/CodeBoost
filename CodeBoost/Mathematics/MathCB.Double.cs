using System.Runtime.CompilerServices;
using CodeBoost.Extensions;

namespace CodeBoost.Mathematics;

/// <summary>
/// Contains various utility methods relating to floating point numbers.
/// </summary>
public static partial class MathCb
{
    /// <summary>
    /// Returns a random value between the supplied minimum and an inclusive maximum.
    /// </summary>
    /// <remarks>
    /// The maximum value is padded with <see cref="double.Epsilon"/> to achieve the closest possibility to inclusive.
    /// </remarks>
    /// <param name="minimum">Inclusive minimum value.</param>
    /// <param name="maximum">Inclusive maximum value.</param>
    /// <returns>A random value between the minimum and the inclusive maximum.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double RandomInclusive(double minimum, double maximum)
    {
        double range = maximum - minimum + double.Epsilon;
        double multiplier = Random.NextDouble() * range;
        return multiplier + minimum;
    }

    /// <summary>
    /// Returns a random value between the supplied minimum and an exclusive maximum.
    /// </summary>
    /// <param name="minimum">Inclusive minimum value.</param>
    /// <param name="maximum">Exclusive maximum value.</param>
    /// <returns>A random value between the minimum and the exclusive maximum.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double RandomExclusive(double minimum, double maximum)
    {
        double range = maximum - minimum;
        double multiplier = Random.NextDouble() * range;
        return multiplier + minimum;
    }

    /// <summary>
    /// Returns a random value between <c>0f</c> and <c>1f</c> inclusive.
    /// </summary>
    /// <returns>A random value between <c>0f</c> and <c>1f</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Random01() => RandomInclusive(0f, 1f);

    /// <summary>
    /// Interpolates between <paramref name="start"/> and <paramref name="end"/> using the supplied percentage.
    /// </summary>
    /// <param name="start">Starting value of the interpolation.</param>
    /// <param name="end">Ending value of the interpolation.</param>
    /// <param name="percentage">Interpolation percentage in the range <c>0</c> to <c>1</c>.</param>
    /// <returns>The interpolated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Lerp(double start, double end, double percentage)
    {
        percentage = percentage.Clamp01();

        return start + (end - start) * percentage;
    }

    /// <summary>
    /// Randomly inverts the sign of the supplied value.
    /// </summary>
    /// <param name="value">Value whose sign may be flipped.</param>
    /// <returns>The original value, or its negation, chosen at random.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double RandomSign(this float value)
    {
        if (RandomInclusive(0UL, 1UL) == 0)
            return value;

        return value * -1d;
    }
}
