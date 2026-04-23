using System;
using System.Buffers;

namespace CodeBoost.Extensions;

public static class ArraySegmentExtensions
{
    /// <summary>
    /// Returns the underlying array to <see cref="ArrayPool{T}.Shared"/> if it is not null.
    /// </summary>
    public static void PoolArrayIntoShared(this ArraySegment<byte> arraySegment)
    {
        if (arraySegment.Array is not null)
            ArrayPool<byte>.Shared.Return(arraySegment.Array);
    }
    
    /// <summary>
    /// Gets a non-allocating enumerator for an ArraySegment.
    /// </summary>
    /// <param name="segment">Value to enumerate.</param>
    /// <typeparam name="T0">Type for the ArraySegment.</typeparam>
    /// <returns>An enumerator which uses the offset and count of the ArraySegment.</returns>
    public static ArraySegmentEnumerator<T0> GetEnumerator<T0>(this ArraySegment<T0> segment) => new(segment);

    /// <summary>
    /// A non-allocating enumerator for ArraySegments.
    /// </summary>
    /// <typeparam name="T0">Type for the ArraySegment.</typeparam>
    public struct ArraySegmentEnumerator<T0>
    {
        /// <summary>
        /// The array of the ArraySegment.
        /// </summary>
        private readonly T0[]? _array;
        /// <summary>
        /// The end of the array according to the ArraySegment.Count and ArraySegment.Offset.
        /// </summary>
        private readonly int _end;
        /// <summary>
        /// The current index being enumerated.
        /// </summary>
        private int _index;

        public ArraySegmentEnumerator(ArraySegment<T0> segment)
        {
            _array = segment.Array;

            if (_array is null)
            {
                _index = -1;
                _end = -1;

                return;
            }

            _index = segment.Offset - 1;
            _end = segment.Offset + segment.Count;
        }

        /// <summary>
        /// Iterates to the next entry.
        /// </summary>
        /// <returns>True if there is another entry to enumerate; otherwise, false.</returns>
        public bool MoveNext()
        {
            _index++;
            return _index < _end;
        }

        /// <summary>
        /// Gets the current entry being enumerated.
        /// </summary>
        public T0? Current => _array is null ? default : _array[_index];
    }
}