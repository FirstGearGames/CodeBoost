using System.Collections.Generic;

namespace CodeBoost.Performance;

/// <summary>
/// Holds one pool's per-thread stack, so a rent and a return on the same thread need no synchronization.
/// </summary>
/// <typeparam name="T0">The pooled type.</typeparam>
/// <remarks>
/// <para>
/// <strong>A thread that dies takes its cached entries with it, and that is deliberate.</strong> This used to carry a finalizer that
/// pushed the stack into the pool's shared global stack, so a retiring thread's entries were harvested rather than collected. That is
/// not sound, and it is not fixable by inspecting the pooled type.
/// </para>
/// <para>
/// The stack reaches its finalizer only once it is unreachable, which means the entries it holds are unreachable too, which means they
/// are eligible for finalization themselves. Finalization order is undefined, so any of them may already have run its own finalizer by
/// the time this one runs. Pushing those into the shared stack resurrects them: the pool then hands a later caller an object that has
/// already torn itself down. A <see cref="System.Timers.Timer"/> harvested this way returns marked disposed and throws
/// <see cref="System.ObjectDisposedException"/> the moment it is started, from a pool reporting itself healthy and a stack trace
/// pointing nowhere near here.
/// </para>
/// <para>
/// Checking whether the pooled type declares a finalizer would not close it either, since an object with none can still hold one that
/// has already been finalized. Nor was the harvest free: it took the global stack's lock on the finalizer thread, where blocking stalls
/// finalization for the whole process. What it bought was the entries cached on a retiring thread, capped at the per-thread maximum, of
/// a type every pool can reconstruct with <c>new()</c>. Letting those be collected is the cheaper side of that trade.
/// </para>
/// </remarks>
public class ThreadLocalStackWrapper<T0>
{
    /// <summary>
    /// The stack for the ThreadLocal.
    /// </summary>
    public readonly Stack<T0> LocalStack = [];
}
