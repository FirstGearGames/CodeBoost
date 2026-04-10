using System.Text;
using CodeBoost.Performance;

namespace CodeBoost.Extensions;

/// <summary>
/// Various utility classes relating to Float.
/// </summary>
public static class UInt8Extensions
{
    /// <summary>
    /// Converts bytes to a string.
    /// </summary>
    public static string ToEncodedString(this byte[] bytes, int offset, int count)
    {
        UTF8Encoding encoding = Utf8EncodingPool.Rent();

        string result = encoding.GetString(bytes, offset, count);

        Utf8EncodingPool.Return(encoding);

        return result;
    }

    /// <summary>
    /// Returns if a flags whole value has part within it.
    /// </summary>
    public static bool FastContains(this byte whole, byte part) => (whole & part) == part;

}