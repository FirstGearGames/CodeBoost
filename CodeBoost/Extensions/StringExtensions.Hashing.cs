namespace CodeBoost.Extensions;

public static partial class StringExtensions
{
    private const uint FnvOffsetBasis32 = 2166136261;
    private const uint FnvPrime32 = 16777619;
    private const ulong FnvOffsetBasis64 = 14695981039346656037;
    private const ulong FnvPrime64 = 1099511628211;

    /// <summary>
    /// Computes a non-cryptographic stable hash code that always returns the same hash for the same string.
    /// This is an implementation of FNV-1 32 bit xor folded to 16 bit.
    /// See https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function.
    /// </summary>
    /// <returns>The stable 16 bit hash of the value.</returns>
    /// <param name = "value"> Text. </param>
    public static ushort GetStableHashUi16(this string value)
    {
        uint hash32 = value.GetStableHashUi32();

        return (ushort)((hash32 >> 16) ^ hash32);
    }

    /// <summary>
    /// Computes a non-cryptographic stable hash code that always returns the same hash for the same string.
    /// This is an implementation of FNV-1 32 bit.
    /// See https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function.
    /// </summary>
    /// <returns>The stable 32 bit hash of the value.</returns>
    /// <param name = "value"> Text. </param>
    public static uint GetStableHashUi32(this string value)
    {
        unchecked
        {
            uint hash = FnvOffsetBasis32;
            for (int i = 0; i < value.Length; i++)
            {
                uint ch = value[i];
                hash *= FnvPrime32;
                hash ^= ch;
            }

            return hash;
        }
    }

    /// <summary>
    /// Computes a non-cryptographic stable hash code that always returns the same hash for the same string.
    /// This is an implementation of FNV-1 64 bit.
    /// See https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function.
    /// </summary>
    /// <returns>The stable 64 bit hash of the value.</returns>
    /// <param name = "value"> Text. </param>
    public static ulong GetStableHashUInt64(this string value)
    {
        unchecked
        {
            ulong hash = FnvOffsetBasis64;
            for (int i = 0; i < value.Length; i++)
            {
                ulong ch = value[i];
                hash *= FnvPrime64;
                hash ^= ch;
            }

            return hash;
        }
    }
}