using System;
using System.Buffers;

namespace CodeBoost.Extensions;

public static class ArrayPoolExtensions
{
    /// <summary>
    /// Changes the number of elements of a rented  array to the specified new size.
    /// </summary>
    /// <param name="pool">The target <see cref="ArrayPool{T}"/> instance to use to resize the array.</param>
    /// <param name="array">The rented <typeparamref name="T"/> array to resize.</param>
    /// <param name="newSize">The size of the new array.</param>
    /// <param name="clearArray">Indicates whether the contents of the array should be cleared before reuse.</param>
    /// <remarks>When this method returns, the caller must not use any references to the old array anymore.</remarks>
    public static void ResizeWithoutClearing<T0>(this ArrayPool<T0> pool, ref T0[] array, int newSize)
    {
        T0[] newArray = pool.Rent(newSize);
            
        int itemsToCopy = Math.Min(array.Length, newSize);
        Array.Copy(array, 0, newArray, 0, itemsToCopy);

        pool.Return(array, clearArray: false);

        array = newArray;
    }
}