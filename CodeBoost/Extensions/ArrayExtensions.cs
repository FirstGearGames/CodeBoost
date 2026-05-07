using System;
using System.Threading;

namespace CodeBoost.Extensions;

public static class ArrayExtensions
{
    /// <summary>
    /// Per-thread randomizer used for shuffling. <see cref="System.Random"/> is not thread-safe; concurrent access from multiple threads on a shared instance can return zero or corrupt internal state.
    /// </summary>
    private static readonly ThreadLocal<Random> Random = new(() => new Random(Interlocked.Increment(ref _randomSeedCounter)));
    /// <summary>
    /// A monotonically increasing counter used to seed each thread's <see cref="Random"/> uniquely.
    /// </summary>
    private static int _randomSeedCounter;

    /// <summary>
    /// Shuffles the collection.
    /// </summary>
    public static void Shuffle<T0>(this T0[] array)
    {
        Random random = Random.Value!;

        int n = array.Length;
        for (int i = 0; i < n - 1; i++)
        {
            int r = i + random.Next(n - i);
            (array[r], array[i]) = (array[i], array[r]);
        }
    }
}