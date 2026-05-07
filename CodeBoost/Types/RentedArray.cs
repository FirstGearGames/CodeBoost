using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace CodeBoost.Types;

/// <summary>
/// An array which belongs to an ArrayPool.
/// </summary>
public readonly struct RentedArray<T0> : IDisposable
{
    /// <summary>
    /// The rented array.
    /// </summary>
    public readonly T0[] Array { get; }
    /// <summary>
    /// The ArrayPool which the array was rented from.
    /// </summary>
    private readonly ArrayPool<T0> _pool;

    /// <summary>
    /// Rents an array using a specified pool.
    /// </summary>
    /// <param name="pool">Pool to use.</param>
    /// <param name="minimumLength">Minimum length the array must be.</param>
    public RentedArray(ArrayPool<T0> pool, int minimumLength)
    {
        _pool = pool;
        Array = pool.Rent(minimumLength);
    }

    /// <summary>
    /// Rents an array using the Shared pool.
    /// </summary>
    /// <param name="minimumLength">Minimum length the array must be.</param>
    public RentedArray(int minimumLength) : this(ArrayPool<T0>.Shared, minimumLength)
    {
    }

    public void Dispose()
    {
        _pool.Return(Array, RuntimeHelpers.IsReferenceOrContainsReferences<T0>());
    }
}