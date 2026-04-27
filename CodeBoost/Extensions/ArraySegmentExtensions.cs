using System;
using System.Buffers;

namespace CodeBoost.Extensions;

/// <summary>
/// Extension methods for working with <see cref="ArraySegment{T}"/> values.
/// </summary>
public static class ArraySegmentExtensions
{
    /// <summary>
    /// Returns the underlying array of the supplied segment to <see cref="ArrayPool{T}.Shared"/> when it is not null.
    /// </summary>
    /// <param name="arraySegment">Segment whose underlying array should be returned to the shared pool.</param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ArraySegmentEnumerator{T0}"/> struct that walks the supplied segment.
        /// </summary>
        /// <param name="segment">Segment to enumerate.</param>
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
        /// Advances the enumerator to the next entry in the segment.
        /// </summary>
        /// <returns>True when another entry is available; otherwise false.</returns>
        public bool MoveNext()
        {
            _index++;
            return _index < _end;
        }

        /// <summary>
        /// The entry currently being enumerated.
        /// </summary>
        public T0? Current => _array is null ? default : _array[_index];
    }
}