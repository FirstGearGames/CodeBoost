using System.Text;
using CodeBoost.Performance;

namespace CodeBoost.Extensions;

/// <summary>
/// Extension methods for transforming and inspecting <see cref="string"/> values.
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// The sentinel value indicating that an index is not found or has not been specified.
    /// </summary>
    public const int UnsetIndex = -1;

    /// <summary>
    /// Returns the supplied value, or an empty string when it is null.
    /// </summary>
    /// <param name="value">Value to inspect.</param>
    /// <returns>The supplied value, or an empty string when it is null.</returns>
    public static string EmptyIfNull(this string? value) => value ?? string.Empty;

    /// <summary>
    /// Converts a member-style string to PascalCase.
    /// </summary>
    /// <remarks>
    /// Leading non-alpha characters are removed and the first alpha character is capitalized.
    /// </remarks>
    /// <param name="value">String to convert.</param>
    /// <returns>The PascalCase representation of the supplied string.</returns>
    public static string MemberToPascalCase(this string value)
    {
        int index = value.GetFirstLetterOrDigitIndex();

        //Index not found. String is null or has no chars/numbers.
        if (index == UnsetIndex)
            return value;

        char firstValidChar = value[index];

        //First character is not a letter, return as-is.
        if (!char.IsLetter(firstValidChar))
            return value;

        //Already capitalized.
        if (char.IsUpper(firstValidChar))
            return value;

        StringBuilder stringBuilder = ObjectPool<StringBuilder>.Rent();
        stringBuilder.Clear();

        stringBuilder.Append(char.ToUpperInvariant(firstValidChar));
        stringBuilder.Append(value.Substring(index + 1));

        string result = stringBuilder.ToString();
        ObjectPool<StringBuilder>.Return(stringBuilder);

        return result;
    }


    /// <summary>
    /// Converts a PascalCase string to member case, optionally prepending a prefix.
    /// </summary>
    /// <remarks>
    /// The prefix is only added when it is not already present at the start of the value.
    /// </remarks>
    /// <example>
    /// With a prefix of <c>_</c> the value <c>HelloWorld</c> is returned as <c>_helloWorld</c>.
    /// </example>
    /// <param name="value">String to convert.</param>
    /// <param name="prefix">Prefix to prepend when not already present.</param>
    /// <returns>The member-case representation of the supplied string.</returns>
    public static string PascalCaseToMember(this string value, string prefix = "_")
    {
        int index = value.GetFirstLetterOrDigitIndex();

        //Index not found. String is null or has no chars/numbers.
        if (index == UnsetIndex)
            return value;

        char firstValidChar = value[index];
        int prefixLength = prefix.Length;
            
        /* There are marginally more efficient ways to handle these prefix operations
         * but allocations are going to occur either way - use what is easier to read. */
        StringBuilder stringBuilder = ObjectPool<StringBuilder>.Rent();
        stringBuilder.Clear();

        //There is a prefix.
        if (prefixLength > 0)
        {
            if (prefixLength >= value.Length)
            {
                stringBuilder.Append(prefix);

                AppendLowerFirstCharAndRemainingValue();

                return stringBuilder.ToString();
            }

            //If prefix is not yet added then do so.
            if (value.Substring(0, prefixLength) != prefix)
                stringBuilder.Append(prefix);
        }

        //Add renaming with lowercase char.
        AppendLowerFirstCharAndRemainingValue();
            
        string result = stringBuilder.ToString();
        ObjectPool<StringBuilder>.Return(stringBuilder);

        return result;

        //Appends lowercase first char, and any renaming text in value.
        void AppendLowerFirstCharAndRemainingValue()
        {
            stringBuilder.Append(char.ToLowerInvariant(firstValidChar));
            //If value has enough length remaining append it as well.
            if (value.Length >= index)
                stringBuilder.Append(value.Substring(index + 1));
        }
    }

    /// <summary>
    /// Converts the supplied string into a byte array rented from the shared pool.
    /// </summary>
    /// <remarks>
    /// The buffer is rented from <see cref="System.Buffers.ArrayPool{T}.Shared"/> and may be larger than the bytes written.
    /// </remarks>
    /// <param name="value">String to convert.</param>
    /// <param name="bytesWritten">Receives the number of bytes written to the buffer.</param>
    /// <returns>The pooled byte array containing the converted string.</returns>
    public static byte[] ToBytesNonAllocated(this string value, out int bytesWritten)
    {
        int valueLength = value.Length;
        UTF8Encoding encoding = Utf8EncodingPool.Rent();

        // Number of minimum bytes the buffer must be.
        int bytesNeeded = encoding.GetMaxByteCount(valueLength);

        byte[] array = System.Buffers.ArrayPool<byte>.Shared.Rent(bytesNeeded);

        bytesWritten = encoding.GetBytes(value, charIndex: 0, valueLength, array, byteIndex: 0);

        Utf8EncodingPool.Return(encoding);

        return array;
    }
        
    /// <summary>
    /// Returns the index of the first letter or digit in the supplied string.
    /// </summary>
    /// <param name="value">String to inspect.</param>
    /// <returns>The zero-based index of the first letter or digit, or <see cref="UnsetIndex"/> when none is found.</returns>
    public static int GetFirstLetterOrDigitIndex(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return UnsetIndex;

        for (int i = 0; i < value.Length; i++)
        {
            if (char.IsLetterOrDigit(value[i]))
                return i;
        }

        return UnsetIndex;
    }
}