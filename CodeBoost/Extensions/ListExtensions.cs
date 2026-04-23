using System;
using System.Collections.Generic;
using CodeBoost.Types;

namespace CodeBoost.Extensions;

public static class ListExtensions
{
    /// <summary>
    /// The randomizer used for shuffling.
    /// </summary>
    private static readonly Random Random = new();

    /// <summary>
    /// Adds an element to the collection if it does not already exist.
    /// </summary>
    /// <returns>True if the item was added; otherwise, false.</returns>
    public static bool AddUnique<T0>(this List<T0> list, T0 value)
    {
        bool contains = list.Contains(value);
        if (!contains)
            list.Add(value);

        return !contains;
    }

    /// <summary>
    /// Removes the first entry from the collection and returns it.
    /// </summary>
    /// <returns>The first entry in the collection, or default if the collection is null or empty.</returns>
    public static T0? RemoveAndReturnFirst<T0>(this List<T0> list)
    {
        if (list is null || list.Count == 0)
            return default;

        T0 firstValue = list[0];
        list.RemoveAt(0);

        return firstValue;
    }

    /// <summary>
    /// Removes the last entry from the collection and returns it.
    /// </summary>
    /// <returns>The last entry in the collection, or default if the collection is null or empty.</returns>
    public static T0 GetAndRemoveLastValue<T0>(this List<T0> list)
    {
        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        #pragma warning disable CS8603 // Possible null reference return.

        int count = list?.Count ?? 0;
        // Indicates a null list, or no entries.
        if (count == 0)
            return default;

        T0 lastValue = list[count - 1];
        list.RemoveAt(count - 1);

        return lastValue;
        #pragma warning restore CS8602 // Dereference of a possibly null reference.
        #pragma warning restore CS8603 // Possible null reference return.
    }
      
    /// <summary>
    /// Shuffles the collection.
    /// </summary>
    public static void Shuffle<T0>(this List<T0> lst)
    {
        int n = lst.Count;
        for (int i = 0; i < n - 1; i++)
        {
            int r = i + Random.Next(n - i);
            (lst[r], lst[i]) = (lst[i], lst[r]);
        }
    }

    /// <summary>
    /// Adds an orderable item to the collection in ascending order.
    /// </summary>
    public static void AddOrderedAscending<T0>(this List<T0> collection, T0 item) where T0 : IOrderable
    {
        int count = collection.Count;
        int itemOrder = item.Order;

        /* If no entries or is equal or larger to last
         * entry then value can be added onto the end. */
        if (count == 0 || itemOrder >= collection[count - 1].Order)
        {
            collection.Add(item);
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                /* If item being sorted is lower than the one in already added.
                 * then insert it before the one already added. */
                if (itemOrder <= collection[i].Order)
                {
                    collection.Insert(i, item);
                    break;
                }
            }
        }
    }
}