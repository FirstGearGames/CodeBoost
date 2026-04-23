using System;
using System.Collections.Generic;
using System.Text;
using CodeBoost.Performance;

namespace CodeBoost.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Casts each item in the collection to a string and returns all values joined by the delimiter.
    /// </summary>
    /// <returns>The concatenated string of all items separated by the delimiter, or an empty string if the collection is null.</returns>
    public static string ToString<T0>(this IEnumerable<T0> thisValue, string delimiter = ", ")
    {
        if (thisValue is null)
            return string.Empty;

        StringBuilder stringBuilder = ObjectPool<StringBuilder>.Rent();
        stringBuilder.Clear();
            
        foreach (T0 item in thisValue)
            stringBuilder.Append(item + delimiter);

        // Remove ending delimiter.
        if (stringBuilder.Length > delimiter.Length)
            stringBuilder.Length -= delimiter.Length;

        string result = stringBuilder.ToString();
        ObjectPool<StringBuilder>.Return(stringBuilder);

        return result;
    }

    /// <summary>
    /// Calls Dispose on each element within the collection.
    /// </summary>
    public static void Dispose<T0>(this IEnumerable<T0> thisValue) where T0 : IDisposable
    {
        if (thisValue is null)
            return;

        foreach (T0 type0 in thisValue)
            type0?.Dispose();
    }

}