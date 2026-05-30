using System;
using System.Threading;

namespace CodeBoost.Mathematics;

/// <summary>
/// Contains various utility methods relating to floating point numbers.
/// </summary>
public static partial class MathCb
{
    /// <summary>
    /// Per-thread random instance. <see cref="System.Random"/> is not thread-safe; concurrent access from multiple threads on a shared instance can return zero or corrupt internal state.
    /// </summary>
    private static readonly ThreadLocal<Random> Random = new(() => new(Interlocked.Increment(ref _randomSeedCounter)));
    /// <summary>
    /// A monotonically increasing counter used to seed each thread's <see cref="Random"/> uniquely. Avoids the time-resolution collision when multiple threads construct <see cref="Random"/> simultaneously.
    /// </summary>
    private static int _randomSeedCounter;
}