using System.Runtime.CompilerServices;

namespace CodeBoost.Mathematics;

/// <summary>
/// Contains various utility methods relating to floating point numbers.
/// </summary>
public static partial class MathCb
{
    /// <summary>
    /// Returns a clamped ulong within a specified range.
    /// </summary>
    /// <param name="value">Value to clamp.</param>
    /// <param name="minimum">Minimum value.</param>
    /// <param name="maximum">Maximum value.</param>
    /// <returns>The value clamped within the specified range.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Clamp(ulong value, ulong minimum, ulong maximum)
    {
        if (value < minimum)
            value = minimum;
        else if (value > maximum)
            value = maximum;

        return value;
    }

    /// <summary>
    /// Returns whichever value is lower.
    /// </summary>
    /// <param name="value">First value to compare.</param>
    /// <param name="other">Second value to compare.</param>
    /// <returns>The lower of the two supplied values.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Min(ulong value, ulong other) => value < other ? value : other;
}
