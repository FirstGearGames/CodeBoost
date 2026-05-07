namespace CodeBoost.Types;

/// <summary>
/// Per-query predicate consumed by <see cref="RingBufferExtensions.TryFindFirst{T0, TMatcher}"/>. Implementations are expected to be <c>struct</c> so the JIT can specialize the generic, inline the <see cref="IsMatch"/> body, and avoid both delegate allocations and virtual dispatch in tight ring walks.
/// </summary>
/// <typeparam name="T0">The element type of the ring buffer being matched against.</typeparam>
public interface IRingBufferMatcher<T0>
{
    /// <summary>
    /// Returns true when <paramref name="value"/> satisfies the matcher's condition.
    /// </summary>
    /// <param name="value">The candidate value.</param>
    bool IsMatch(T0 value);
}
