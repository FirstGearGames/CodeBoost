using System;
using System.Runtime.CompilerServices;
using CodeBoost.Extensions;

namespace CodeBoost.Mathematics;

/// <summary>
/// Contains various utility methods relating to floating point numbers.
/// </summary>
public static partial class MathCb
{
    /// <summary>
    /// Converts a single to a UInt32 using ZigZag encoding, with round-to-nearest semantics.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <returns>The converted UInt32 value.</returns>
    /// <remarks>
    /// Unsafe in the range sense: <paramref name="value"/> scaled by <paramref name="accuracy"/> must land inside <see cref="int"/>,
    /// and keeping it there is the caller's business. One that does not is converted however the platform converts it, which is the
    /// trade the name is offering. There is deliberately no clamp, since a caller wanting one wants it at its own boundary rather than
    /// on every conversion this performs.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint SingleToUInt32Unsafe(double value, float accuracy)
    {
        return ((int)Math.Round(value * (1d / accuracy), MidpointRounding.AwayFromZero)).ToUInt32();
    }
}
