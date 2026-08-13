using System.Runtime.CompilerServices;
using System.Text;
using CodeBoost.Performance;

namespace CodeBoost.Extensions;

/// <summary>
/// Various utility classes relating to Float.
/// </summary>
public static class UInt8Extensions
{
    /// <summary>
    /// Clamps the supplied value into the inclusive range from <paramref name="minimum"/> to <paramref name="maximum"/>.
    /// </summary>
    /// <param name="value">Value to clamp.</param>
    /// <param name="minimum">Inclusive minimum value.</param>
    /// <param name="maximum">Inclusive maximum value.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Clamp(this byte value, byte minimum, byte maximum)
    {
        if (value < minimum)
            return minimum;

        if (value > maximum)
            return maximum;

        return value;
    }

    /// <summary>
    /// Converts the bytes to a UTF-8 encoded string.
    /// </summary>
    public static string ToEncodedString(this byte[] bytes, int offset, int count)
    {
        UTF8Encoding encoding = Utf8EncodingPool.Rent();

        string result = encoding.GetString(bytes, offset, count);

        Utf8EncodingPool.Return(encoding);

        return result;
    }

    /// <summary>
    /// Returns whether the whole flags value contains the specified part.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FastContains(this byte whole, byte part) => (whole & part) == part;
}
