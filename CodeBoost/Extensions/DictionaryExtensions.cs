using System.Collections.Generic;
using CodeBoost.Performance;

namespace CodeBoost.Extensions;

public static class DictionaryExtensions
{
    /// <summary>
    /// Returns the values of the dictionary as a list.
    /// </summary>
    /// <remarks>The returned list is taken from a collection pool.</remarks>
    public static List<T1> ValuesToList<T0, T1>(this IDictionary<T0, T1> dict)
    {
        List<T1> result = ListPool<T1>.Rent();
        dict.ValuesToList(ref result);

        return result;
    }

    /// <summary>
    /// Clears a list and populates it with the values of a dictionary.
    /// </summary>
    public static void ValuesToList<T0, T1>(this IDictionary<T0, T1> dict, ref List<T1> result)
    {
        result.Clear();
        foreach (T1 item in dict.Values)
            result.Add(item);
    }

    /// <summary>
    /// Returns the keys of the dictionary as a list.
    /// </summary>
    /// <remarks>The returned list is taken from a collection pool.</remarks>
    public static List<T0> KeysToList<T0, T1>(this IDictionary<T0, T1> dict)
    {
        List<T0> result = ListPool<T0>.Rent();
        dict.KeysToList(ref result);

        return result;
    }

    /// <summary>
    /// Clears a list and populates it with the keys of a dictionary.
    /// </summary>
    public static void KeysToList<T0, T1>(this IDictionary<T0, T1> dict, ref List<T0> result)
    {
        result.Clear();
        foreach (T0 item in dict.Keys)
            result.Add(item);
    }
}