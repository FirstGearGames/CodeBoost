namespace CodeBoost.Types;

/// <summary>
/// Snapshot of a <see cref="RingBuffer{T0}"/>'s underlying state captured in one call so a tight-loop walker can iterate the buffer without paying for repeated property reads or per-step <c>GetRealIndex</c> dispatch.
/// </summary>
/// <typeparam name="T0">The element type of the ring buffer.</typeparam>
public readonly struct RingBufferWalkState<T0>
{
    /// <summary>
    /// The backing array; index this directly using <see cref="BaseRealIndex"/>, advancing by one and wrapping at <see cref="Capacity"/>.
    /// </summary>
    public readonly T0[] Collection;
    /// <summary>
    /// The number of entries currently written.
    /// </summary>
    public readonly int Count;
    /// <summary>
    /// The maximum size of the collection; used as the wrap point when advancing the real index.
    /// </summary>
    public readonly int Capacity;
    /// <summary>
    /// The real index of simulated index 0; advance forward and wrap at <see cref="Capacity"/>.
    /// </summary>
    public readonly int BaseRealIndex;
    /// <summary>
    /// The real index of the newest entry (simulated index <see cref="Count"/> - 1). Undefined when <see cref="Count"/> is zero; callers must gate on <see cref="Count"/> before using this value.
    /// </summary>
    public readonly int LastRealIndex;

    /// <summary>
    /// Constructs the snapshot.
    /// </summary>
    public RingBufferWalkState(T0[] collection, int count, int capacity, int baseRealIndex, int lastRealIndex)
    {
        Collection = collection;
        Count = count;
        Capacity = capacity;
        BaseRealIndex = baseRealIndex;
        LastRealIndex = lastRealIndex;
    }
}
