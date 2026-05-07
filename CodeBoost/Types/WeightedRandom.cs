using System.Collections.Generic;
using CodeBoost.Logging;
using CodeBoost.Mathematics;
using CodeBoost.Performance;

namespace CodeBoost.Types;

public static class Weighted
{
    /// <summary>
    /// Returns random values selected by weight.
    /// </summary>
    /// <param name = "source"> values to pick from. </param>
    /// <param name = "count"> Number of entries to get. </param>
    /// <param name = "results"> Results of entries. Key is the entry, Value is the number of times the entry was picked. </param>
    /// <param name = "allowRepeatingEntries"> True to allow the same entry to be picked more than once. </param>
    public static void GetValues<T0>(List<T0> source, uint count, ref Dictionary<T0, uint> results, bool allowRepeatingEntries = false) where T0 : IWeighted => GetValues(source, new UIntRange(count, count), ref results, allowRepeatingEntries);

    /// <summary>
    /// Returns random values selected by weight.
    /// </summary>
    /// <param name = "source"> values to pick from. </param>
    /// <param name = "countRange"> Number of entries to get. </param>
    /// <param name = "results"> Results of entries. Key is the entry, Value is the number of times the entry was picked. </param>
    /// <param name = "allowRepeatingEntries"> True to allow the same entry to be picked more than once. </param>
    public static void GetValues<T0>(List<T0> source, UIntRange countRange, ref Dictionary<T0, uint> results, bool allowRepeatingEntries = false) where T0 : IWeighted
    {
        if (source is null)
        {
            Logger.LogError($"Source list of type {typeof(T0).Name} cannot be null.");
            return;
        }

        int sourceCount = source.Count;

        if (sourceCount == 0)
            return;

        uint addCount = (uint)MathCb.RandomInclusive(countRange.Minimum, countRange.Maximum);
        // If to not return any then exit early.
        if (addCount == 0)
            return;

        // Get the total weight.
        float totalWeight = 0f;
        for (int i = 0; i < sourceCount; i++)
            totalWeight += source[i].GetWeight();

        //Indexes which can no longer be checked.
        HashSet<int> exhaustedIndexes = HashSetPool<int>.Rent();

        uint totalPicked = 0;
        while (totalPicked < addCount)
        {
            // All entries have reached their quantity limits — nothing left to pick.
            if (exhaustedIndexes.Count >= sourceCount)
                break;

            // Pick a random weight along the cumulative distribution.
            float rnd = (float)MathCb.RandomInclusive(0f, totalWeight);
            bool isPicked = false;

            for (int i = 0; i < sourceCount; i++)
            {
                // Entry cannot be iterated anymore due to quantity limits on it.
                if (exhaustedIndexes.Contains(i))
                    continue;

                T0 item = source[i];
                float weight = item.GetWeight();

                rnd -= weight;
                if (rnd > 0f)
                    continue;

                // Try to get current count.
                results.TryGetValue(item, out uint currentCount);

                uint newCount = currentCount + 1;
                // Update count for item.
                results[item] = newCount;
                totalPicked++;
                isPicked = true;

                /* If item cannot be allowed more counts
                 * then remove it possible results
                 * and update weight to be without
                 * the items weight. */
                if (!allowRepeatingEntries || newCount >= item.GetQuantityRange().Maximum)
                {
                    exhaustedIndexes.Add(i);
                    totalWeight -= weight;
                }

                break;
            }

            /* If nothing was picked then either floating-point drift
             * left rnd above the remaining weight or every remaining
             * weight is zero. Either way, no further progress is possible. */
            if (!isPicked)
                break;
        }

        HashSetPool<int>.Return(exhaustedIndexes);
    }
}