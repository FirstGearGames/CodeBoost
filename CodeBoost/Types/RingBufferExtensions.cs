using System.Runtime.CompilerServices;

namespace CodeBoost.Types;

/// <summary>
/// Reusable lookup helpers over <see cref="RingBuffer{T0}"/> that consume the buffer's hoisted walk state instead of per-step indexer dispatch.
/// </summary>
public static class RingBufferExtensions
{
    /// <summary>
    /// Outputs the newest entry without going through the simulated-index path.
    /// </summary>
    /// <typeparam name="T0">The element type.</typeparam>
    /// <param name="buffer">The ring buffer to peek.</param>
    /// <param name="value">The newest entry on success; <c>default</c> when the buffer is empty.</param>
    /// <returns>True when an entry was peeked.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryPeekLast<T0>(this RingBuffer<T0> buffer, out T0 value)
    {
        RingBufferWalkState<T0> state = buffer.GetWalkState();
        if (state.Count == 0)
        {
            value = default!;
            return false;
        }

        value = state.Collection[state.LastRealIndex];
        return true;
    }

    /// <summary>
    /// Walks the ring forward from the oldest entry and outputs the first one that satisfies <paramref name="matcher"/>.
    /// </summary>
    /// <typeparam name="T0">The element type.</typeparam>
    /// <typeparam name="TMatcher">The matcher type. Constrained to <c>struct</c> so the JIT specializes the generic and inlines <see cref="IRingBufferMatcher{T0}.IsMatch"/>.</typeparam>
    /// <param name="buffer">The ring buffer to walk.</param>
    /// <param name="matcher">The matcher used to test each entry.</param>
    /// <param name="value">The matched entry on success; <c>default</c> when no entry matched.</param>
    /// <param name="simulatedIndex">The simulated index of the matched entry on success; -1 when no entry matched.</param>
    /// <returns>True when an entry matched.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryFindFirst<T0, TMatcher>(this RingBuffer<T0> buffer, ref TMatcher matcher, out T0 value, out int simulatedIndex)
        where TMatcher : struct, IRingBufferMatcher<T0>
    {
        RingBufferWalkState<T0> state = buffer.GetWalkState();
        return TryFindFirst(in state, ref matcher, out value, out simulatedIndex);
    }

    /// <summary>
    /// Walks the ring forward from the oldest entry and outputs the first one that satisfies <paramref name="matcher"/>. Use this overload when the caller already holds a hoisted <see cref="RingBufferWalkState{T0}"/> so multiple lookups share one state read.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryFindFirst<T0, TMatcher>(in RingBufferWalkState<T0> state, ref TMatcher matcher, out T0 value, out int simulatedIndex)
        where TMatcher : struct, IRingBufferMatcher<T0>
    {
        int count = state.Count;
        int capacity = state.Capacity;
        T0[] collection = state.Collection;

        int real = state.BaseRealIndex;
        for (int i = 0; i < count; i++)
        {
            if (matcher.IsMatch(collection[real]))
            {
                value = collection[real];
                simulatedIndex = i;
                return true;
            }

            real++;
            if (real >= capacity)
                real = 0;
        }

        value = default!;
        simulatedIndex = -1;
        return false;
    }
}
