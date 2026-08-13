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
    /// Converts a single to a UInt32 using ZigZag encoding after clamping into the range of <see cref="int"/> with round-to-nearest semantics.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="accuracy">Accuracy to use for decimals. This value is typically less than <c>1f</c>.</param>
    /// <returns>The converted UInt32 value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint SingleToUInt32Unsafe(double value, float accuracy)
    {
        /* Rounded into a long before the clamp, not after. Casting to int first made the clamp a no-op, since an int is always
         * within int range, and left a value outside that range to an unspecified double-to-int conversion. */
        long wholeValue = (long)Math.Round(value * (1d / accuracy), MidpointRounding.AwayFromZero);

        return ((int)Polyfill.Clamp(wholeValue, int.MinValue, int.MaxValue)).ToUInt32();
    }
}
