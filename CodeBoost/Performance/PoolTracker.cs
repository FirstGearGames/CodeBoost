using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CodeBoost.Performance;

/// <summary>
/// Catches an object or array returned to a pool it is already in, and reports pooled objects that were rented and never returned.
/// </summary>
/// <remarks>
/// <para>
/// A pool stores a second return of the same instance twice, so two later renters share one instance and corrupt each other far from the faulty return.
/// While tracking is enabled, that second return throws instead and never reaches the pool.
/// </para>
/// <para>
/// Tracking is on by default in a Debug build of CodeBoost and off in Release, where the pools skip it after one check. Release can switch it on at run time, which is how the Release suite uses it.
/// </para>
/// <para>
/// This state is static because the pools it watches are process-wide, so the instances it tracks have no owning instance.
/// Call <see cref="EnableTracking"/> at the start of each run and <see cref="DisableTracking"/> at its end.
/// A leak check sees every rental in the process, so run it only once every peer sharing the process has shut down.
/// </para>
/// </remarks>
public static class PoolTracker
{
    /// <summary>
    /// True while rentals and returns are being tracked.
    /// </summary>
    public static bool TrackingEnabled { get; private set; } = TrackingEnabledByDefault;

    /// <summary>
    /// The instances currently sitting in a pool.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Held weakly, so the tracking never keeps alive an instance its pool has let go of: one cached on a thread that has since exited, or an array its array pool dropped without saying so.
    /// An instance still in a pool is held by that pool, so it stays in this table for as long as a second return of it could reach the pool.
    /// </para>
    /// <para>
    /// Replaced rather than cleared, because netstandard2.0 has no <c>ConditionalWeakTable.Clear</c>.
    /// </para>
    /// </remarks>
    private static ConditionalWeakTable<object, object> _pooledInstances = new();
    /// <summary>
    /// Guards both tracking sets.
    /// </summary>
    private static readonly object TrackedInstancesLock = new();
    /// <summary>
    /// The value stored against each instance in <see cref="_pooledInstances"/>; only the instance's presence matters.
    /// </summary>
    private static readonly object PooledMarker = new();
    /// <summary>
    /// The instances rented while tracking was enabled and not yet returned.
    /// </summary>
    private static readonly HashSet<object> RentedInstances = new(ReferenceComparer.Instance);

    #if DEBUG
    /// <summary>
    /// The value <see cref="TrackingEnabled"/> starts with; a Debug build tracks from process start.
    /// </summary>
    private const bool TrackingEnabledByDefault = true;
    #else
    /// <summary>
    /// The value <see cref="TrackingEnabled"/> starts with; a Release build skips tracking until it is enabled.
    /// </summary>
    private const bool TrackingEnabledByDefault = false;
    #endif

    /// <summary>
    /// Clears any previous tracking and starts tracking rentals and returns.
    /// </summary>
    public static void EnableTracking()
    {
        lock (TrackedInstancesLock)
        {
            _pooledInstances = new();
            RentedInstances.Clear();
            TrackingEnabled = true;
        }
    }

    /// <summary>
    /// Stops tracking and clears everything tracked.
    /// </summary>
    public static void DisableTracking()
    {
        lock (TrackedInstancesLock)
        {
            TrackingEnabled = false;
            _pooledInstances = new();
            RentedInstances.Clear();
        }
    }

    /// <summary>
    /// Gets the number of instances rented while tracking was enabled that have not been returned.
    /// </summary>
    /// <returns>The number of outstanding rentals.</returns>
    public static int GetRentedCount()
    {
        lock (TrackedInstancesLock)
            return RentedInstances.Count;
    }

    /// <summary>
    /// Throws when any instance rented while tracking was enabled has not been returned.
    /// </summary>
    /// <remarks>Call this at teardown, once everything that rents has shut down.</remarks>
    public static void ThrowIfAnyRented()
    {
        lock (TrackedInstancesLock)
        {
            if (RentedInstances.Count == 0)
                return;

            /* Allocated rather than rented: a rented list would be recorded in the very set it describes, and is reported as a leak
             * of its own. This runs once, at teardown, just before throwing. */
            List<string> descriptions = new(RentedInstances.Count);

            foreach (object instance in RentedInstances)
                descriptions.Add(Describe(instance));

            throw new InvalidOperationException($"{RentedInstances.Count} pooled instances were never returned: {string.Join(", ", descriptions)}.");
        }
    }

    /// <summary>
    /// Records an instance as rented and no longer in its pool.
    /// </summary>
    /// <param name="instance">The rented instance.</param>
    internal static void RecordRented(object instance)
    {
        lock (TrackedInstancesLock)
        {
            _pooledInstances.Remove(instance);
            RentedInstances.Add(instance);
        }
    }

    /// <summary>
    /// Records an instance as returned to its pool, throwing when it is already there.
    /// Call this whether the pool keeps the instance or discards it, since either way the rental has ended.
    /// </summary>
    /// <param name="instance">The instance being pooled.</param>
    internal static void RecordPooled(object instance)
    {
        lock (TrackedInstancesLock)
        {
            if (_pooledInstances.TryGetValue(instance, out _))
                throw new InvalidOperationException($"{Describe(instance)} was returned to its pool while already in it.");

            _pooledInstances.Add(instance, PooledMarker);
            RentedInstances.Remove(instance);
        }
    }

    /// <summary>
    /// Describes an instance by its type, and by its length when it is an array.
    /// </summary>
    /// <param name="instance">The instance to describe.</param>
    /// <returns>The description.</returns>
    private static string Describe(object instance) => instance is Array array ? $"{array.GetType().GetElementType()!.Name}[{array.Length}]" : instance.GetType().Name;

    /// <summary>
    /// Compares instances by reference, so a type overriding equality is still tracked per instance.
    /// </summary>
    private sealed class ReferenceComparer : IEqualityComparer<object>
    {
        /// <summary>
        /// The shared comparer.
        /// </summary>
        public static readonly ReferenceComparer Instance = new();

        /// <inheritdoc/>
        public new bool Equals(object x, object y) => ReferenceEquals(x, y);

        /// <inheritdoc/>
        public int GetHashCode(object instance) => RuntimeHelpers.GetHashCode(instance);
    }
}
