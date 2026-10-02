using System;
using System.Buffers;

namespace CodeBoost.Performance;

/// <summary>
/// Rents arrays from <see cref="ArrayPool{T}.Shared"/> and, while <see cref="PoolTracker"/> is tracking, catches an array returned while already in the pool or one never returned.
/// </summary>
/// <remarks>
/// <see cref="ArrayPool{T}"/> does not detect a second return of the same array. It stores the array twice, so two later renters share one buffer and corrupt each other far from the faulty return.
/// The check only holds when every rent and return of an array goes through this type; one returned straight to <see cref="ArrayPool{T}.Shared"/> is never recorded.
/// </remarks>
public static class TrackedArrayPool<T0>
{
    /// <summary>
    /// Rents an array of at least the specified length.
    /// </summary>
    /// <param name="minimumLength">The minimum length the array must be.</param>
    /// <returns>An array whose length is at least <paramref name="minimumLength"/>.</returns>
    public static T0[] Rent(int minimumLength)
    {
        T0[] array = ArrayPool<T0>.Shared.Rent(minimumLength);

        if (PoolTracker.TrackingEnabled && array.Length > 0)
            PoolTracker.RecordRented(array);

        return array;
    }

    /// <summary>
    /// Replaces a rented array with a rented array of the specified size, copying across as many elements as both hold, and returns the old array without clearing it.
    /// </summary>
    /// <param name="array">The rented array to resize, which is replaced by the new array.</param>
    /// <param name="newSize">The minimum length of the new array.</param>
    /// <remarks>When this method returns, the caller must not use any references to the old array anymore.</remarks>
    public static void ResizeWithoutClearing(ref T0[] array, int newSize)
    {
        T0[] newArray = Rent(newSize);

        int copyCount = Math.Min(array.Length, newSize);
        Array.Copy(array, 0, newArray, 0, copyCount);

        Return(array, clearArray: false);

        array = newArray;
    }

    /// <summary>
    /// Returns an array to the pool and sets the provided reference to null.
    /// This method will not execute if the array is null.
    /// </summary>
    /// <param name="array">The array to return.</param>
    /// <param name="clearArray">True to clear the contents of the array before it is reused.</param>
    public static void ReturnAndNullifyReference(ref T0[] array, bool clearArray = false)
    {
        Return(array, clearArray);

        array = null;
    }

    /// <summary>
    /// Returns an array to the pool.
    /// This method will not execute if the array is null.
    /// </summary>
    /// <param name="array">The array to return.</param>
    /// <param name="clearArray">True to clear the contents of the array before it is reused.</param>
    /// <remarks>While tracking, an array already in the pool throws and never reaches it, so the pool is not corrupted by the faulty return.</remarks>
    public static void Return(T0[] array, bool clearArray = false)
    {
        if (array is null)
            return;

        //An empty array is the one shared instance every zero-length rent hands out, and the pool never keeps it.
        if (PoolTracker.TrackingEnabled && array.Length > 0)
            PoolTracker.RecordPooled(array);

        ArrayPool<T0>.Shared.Return(array, clearArray);
    }
}
