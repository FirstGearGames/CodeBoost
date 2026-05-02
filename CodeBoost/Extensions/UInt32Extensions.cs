using System;
using System.Runtime.CompilerServices;
using CodeBoost.Logging;
using CodeBoost.Mathematics;

namespace CodeBoost.Extensions;

public static class UInt32Extensions
{
    /// <summary>
    /// Converts a UInt32 to an Int32 using ZigZag encoding.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32(this uint value) => (int)((value >> 1) ^ (~(value & 1) + 1));
    
    /// <summary>
    /// Divides the supplied value by the divisor using the requested rounding strategy.
    /// </summary>
    /// <param name="value">Value to divide.</param>
    /// <param name="divisor">Divisor to divide the value by.</param>
    /// <param name="roundingType">Rounding type to use.</param>
    /// <returns>The rounded quotient of the division, or <c>0</c> when the divisor is <c>0</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Divide(this uint value, uint divisor, RoundingType roundingType)
    {
        if (divisor == 0)
        {
            Logger.LogError(typeof(UInt32Extensions), $"The divisor cannot be 0.");
            return 0;
        }

        switch (roundingType)
        {
            case RoundingType.ToEven:
                return (uint)Math.Round((float)value / divisor, MidpointRounding.ToEven);
            case RoundingType.AwayFromZero:
                return (value + divisor / 2) / divisor;
            case RoundingType.Down:
                return value / divisor;
            case RoundingType.Up:
                return value == 0 ? 0 : (value + divisor - 1) / divisor;
            case RoundingType.UpNonZero:
                return value == 0 ? 1 : (value + divisor - 1) / divisor;
            default:
                Logger.LogError(typeof(UInt32Extensions), $"The rounding type [{roundingType}] is unhandled.");
                return 0;
        }
    }

    /// <summary>
    /// Returns whether every value in the supplied array is equal to the first value.
    /// </summary>
    /// <remarks>
    /// True is returned when the array is null or empty.
    /// </remarks>
    /// <param name="values">Values to compare.</param>
    /// <returns>True when every value matches the first.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreValuesMatching(this uint[] values)
    {
        if (values is null)
            return true;

        if (values.Length == 0)
            return true;

        uint firstValue = values[0];

        for (int i = 1; i < values.Length; i++)
        {
            if (firstValue != values[i])
                return false;
        }

        return true;
    }

    /// <summary>
    /// Returns whether the whole flags value contains the specified part.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FastContains(this uint whole, uint part) => (whole & part) == part;
}
