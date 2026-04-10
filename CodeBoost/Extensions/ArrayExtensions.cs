using System;
using System.Buffers;
using System.Text;
using CodeBoost.Performance;

namespace CodeBoost.Extensions;

public static class ArrayExtensions
{
    /// <summary>
    /// Randomizer used for shuffling.
    /// </summary>
    private static readonly Random Random = new();
        
    /// <summary>
    /// Shuffles a collection.
    /// </summary>
    public static void Shuffle<T0>(this T0[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n - 1; i++)
        {
            int r = i + Random.Next(n - i);
            (array[r], array[i]) = (array[i], array[r]);
        }
    }
}