using System.Text;
using CodeBoost.Performance;

namespace CodeBoost.Extensions;

/// <summary>
/// Various utility classes relating to Float.
/// </summary>
public static class UInt8Extensions
{
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
    public static bool FastContains(this byte whole, byte part) => (whole & part) == part;

}