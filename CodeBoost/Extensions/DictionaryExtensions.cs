using System.Collections.Generic;
using CodeBoost.Performance;

namespace CodeBoost.Extensions;

public static class DictionaryExtensions
{
    /// <summary>
    /// Returns values as a list.
    /// </summary>
    /// <remarks>The returned list is taken from a collection pool.</remarks>
    public static List<TValue> ValuesToList<TKey, TValue>(this IDictionary<TKey, TValue> dict)
    {
        List<TValue> result = ListPool<TValue>.Rent();
        dict.ValuesToList(ref result);

        return result;
    }

    /// <summary>
    /// Clears a list and populates it with the values of a dictionary.
    /// </summary>
    public static void ValuesToList<TKey, TValue>(this IDictionary<TKey, TValue> dict, ref List<TValue> result)
    {
        result.Clear();
        foreach (TValue item in dict.Values)
            result.Add(item);
    }

    /// <summary>
    /// Returns keys as a list.
    /// </summary>
    /// <remarks>The returned list is taken from a collection pool.</remarks>
    public static List<TValue> KeysToList<TKey, TValue>(this IDictionary<TKey, TValue> dict)
    {
        List<TValue> result = ListPool<TValue>.Rent();
        dict.ValuesToList(ref result);

        return result;
    }

    /// <summary>
    /// Clears a list and populates it with the keys of a dictionary.
    /// </summary>
    public static void KeysToList<TKey, TValue>(this IDictionary<TKey, TValue> dict, ref List<TKey> result)
    {
        result.Clear();
        foreach (TKey item in dict.Keys)
            result.Add(item);
    }
}