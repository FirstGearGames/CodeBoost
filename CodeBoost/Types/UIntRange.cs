using System;
using System.Runtime.CompilerServices;
using CodeBoost.Mathematics;

namespace CodeBoost.Types;

[Serializable]
public struct UIntRange
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UIntRange(uint minimum, uint maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary>
    /// The minimum value of the range.
    /// </summary>
    public uint Minimum;
    /// <summary>
    /// The maximum value of the range.
    /// </summary>
    public uint Maximum;

    /// <summary>
    /// Returns an exclusive random value between Minimum and Maximum.
    /// </summary>
    /// <returns> A random value greater than or equal to Minimum and less than Maximum. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint RandomExclusive() => (uint)MathCb.RandomExclusive(Minimum, Maximum);

    /// <summary>
    /// Returns an inclusive random value between Minimum and Maximum.
    /// </summary>
    /// <returns> A random value greater than or equal to Minimum and less than or equal to Maximum. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint RandomInclusive() => (uint)MathCb.RandomInclusive(Minimum, Maximum);

    /// <summary>
    /// Returns true if the value is within the range of Minimum and Maximum.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool InRange(uint value) => value >= Minimum && value <= Maximum;
}