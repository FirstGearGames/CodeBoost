namespace CodeBoost.Extensions;

public static class UInt64Extensions
{
    /// <summary>
    /// The number of bits to use for calculating rounding up, such as to bytes or aligning to bytes.
    /// </summary>
    private const int RoundUpToByteBitCount = 7;

    /// <summary>
    /// Returns how many packed bytes the specified number of bits requires.
    /// </summary>
    /// <returns>The number of bytes required to pack bitCount.</returns>
    /// <remarks>When a 0 <see cref="bitCount"/> is provided the returned value will also be 0.</remarks>
    public static uint ToPackedByteCount(this ulong bitCount) => (uint)(bitCount + 7) / 8;

    /// <summary>
    /// Converts a UInt64 to an Int64 using ZigZag encoding.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    public static long ConvertToInt64(this ulong value) => (long)((value >> 1) ^ (~(value & 1) + 1));

    /// <summary>
    /// Returns whether the whole flags value contains the specified part.
    /// </summary>
    public static bool FastContains(this ulong whole, ulong part) => (whole & part) == part;
}