using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CodeBoost.Logging;
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8601 // Possible null reference assignment.

namespace CodeBoost.Types;

/// <summary>
/// Writes values to a collection of a set size, overwriting old values as needed.
/// </summary>
public class RingBuffer<T0> : IEnumerable<T0>
{
    /// <summary>
    /// A custom enumerator used to prevent garbage collection.
    /// </summary>
    public struct Enumerator : IEnumerator<T0>
    {
        /// <summary>
        /// The current entry in the enumerator.
        /// </summary>
        public T0 Current { get; private set; }

        /// <summary>
        /// The rolling collection to use.
        /// </summary>
        private RingBuffer<T0> _enumeratedRingBuffer;
        /// <summary>
        /// The collection to iterate.
        /// </summary>
        private T0[] _collection;
        /// <summary>
        /// The number of entries read during the enumeration.
        /// </summary>
        private int _entriesEnumerated;
        /// <summary>
        /// The start index of enumerations.
        /// </summary>
        private int _startIndex;
        /// <summary>
        /// The capacity of the enumerated collection, captured during initialization.
        /// </summary>
        private int _capacity;
        /// <summary>
        /// True if currently enumerating.
        /// </summary>
        private bool Enumerating => _enumeratedRingBuffer is not null;
        /// <summary>
        /// The count of the collection during initialization.
        /// </summary>
        private int _initializeCollectionCount;

        public void Initialize(RingBuffer<T0> ringBuffer)
        {
            // if none are written then return.
            if (ringBuffer.Count == 0)
                return;

            _entriesEnumerated = 0;
            _startIndex = ringBuffer.GetRealIndex(0);
            _enumeratedRingBuffer = ringBuffer;
            _collection = ringBuffer.Collection;
            _capacity = ringBuffer.Capacity;
            _initializeCollectionCount = ringBuffer.Count;
            Current = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (!Enumerating)
                return false;

            int written = _enumeratedRingBuffer.Count;

            if (written != _initializeCollectionCount)
            {
                Logger<RingBuffer<T0>>.LogError($"Collection was modified during enumeration.");
                Reset();

                return false;
            }

            if (_entriesEnumerated >= written)
            {
                Reset();

                return false;
            }

            int index = _startIndex + _entriesEnumerated;
            if (index >= _capacity)
                index -= _capacity;
            Current = _collection[index];

            _entriesEnumerated++;

            return true;
        }

        /// <summary>
        /// Resets the read count.
        /// </summary>
        public void Reset()
        {
            /* Only need to reset value types.
             * Numeric types change during initialization. */
            _enumeratedRingBuffer = default;
            _collection = default;
            Current = default;
        }

        object IEnumerator.Current => Current;
        public void Dispose() { }
    }

    /// <summary>
    /// The current write index of the collection.
    /// </summary>
    public int WriteIndex { get; private set; }
    /// <summary>
    /// The number of entries currently written.
    /// </summary>
    public int Count => _written;
    /// <summary>
    /// The maximum size of the collection.
    /// </summary>
    public int Capacity;
    /// <summary>
    /// The collection being used.
    /// </summary>
    public T0[] Collection = [];
    /// <summary>
    /// True if initialized.
    /// </summary>
    public bool Initialized { get; private set; }

    /// <summary>
    /// The number of entries written. This will never go beyond the capacity but will be less until the capacity is filled.
    /// </summary>
    private int _written;
    /// <summary>
    /// The enumerator for the collection.
    /// </summary>
    private Enumerator _enumerator;

    /// <summary>
    /// The default capacity when none is specified.
    /// </summary>
    public const int DefaultCapacity = 60;

    /// <summary>
    /// Initializes with default capacity.
    /// </summary>
    public RingBuffer()
    {
        Initialize(DefaultCapacity);
    }

    /// <summary>
    /// Initializes with a set capacity.
    /// </summary>
    /// <param name = "capacity"> Size to initialize the collection as. This cannot be changed after initialized. </param>
    public RingBuffer(int capacity)
    {
        Initialize(capacity);
    }

    /// <summary>
    /// Initializes the collection at length.
    /// </summary>
    /// <param name = "capacity"> Size to initialize the collection as. This cannot be changed after initialized. </param>
    public void Initialize(int capacity)
    {
        if (capacity <= 0)
        {
            Logger<RingBuffer<T0>>.LogError("Collection length must be larger than 0.");
            return;
        }

        if (Collection is null)
        {
            Collection = ArrayPool<T0>.Shared.Rent(capacity);
        }
        else if (Collection.Length < capacity)
        {
            ArrayPool<T0>.Shared.Return(Collection, Polyfill.IsReferenceOrContainsReferences<T0>());
            Collection = ArrayPool<T0>.Shared.Rent(capacity);
        }

        Capacity = capacity;

        Clear();

        Initialized = true;
    }

    /// <summary>
    /// Initializes with default capacity.
    /// </summary>
    /// <param name = "log"> True to log automatic initialization. </param>
    public void Initialize()
    {
        if (!Initialized)
        {
            Logger<RingBuffer<T0>>.LogInformation($"Instance has been initialized with a default capacity of [{DefaultCapacity}].");
            Initialize(DefaultCapacity);
        }
    }

    /// <summary>
    /// Clears the collection to default values and resets indexing.
    /// </summary>
    public void Clear()
    {
        Array.Clear(Collection, 0, Capacity);

        _written = 0;
        WriteIndex = 0;
        _enumerator.Reset();
    }

    /// <summary>
    /// Resets the write indices and enumerator state without touching the underlying array. Callers that have already nulled their populated slots use this to skip the redundant <see cref="Array.Clear(System.Array, int, int)"/> in <see cref="Clear"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResetWriteState()
    {
        _written = 0;
        WriteIndex = 0;
        _enumerator.Reset();
    }

    /// <summary>
    /// Inserts an entry into the collection.
    /// This can be an expensive operation on larger buffers.
    /// </summary>
    /// <param name = "simulatedIndex"> Simulated index to return. A value of 0 would return the first simulated index in the collection. </param>
    /// <param name = "data"> Data to insert. </param>
    public T0 Insert(int simulatedIndex, T0 data)
    {
        int written = _written;

        // Insert at the end (or into an empty buffer) is an append.
        if (simulatedIndex == written)
            return Add(data);

        int realIndex = GetRealIndex(simulatedIndex);
        if (realIndex == -1)
            return default;

        int capacity = Capacity;
        int lastSimulatedIndex = written == capacity ? written - 1 : written;

        int lRealIndex = capacity - written + lastSimulatedIndex + WriteIndex;
        while (lRealIndex >= capacity)
            lRealIndex -= capacity;

        while (lastSimulatedIndex > simulatedIndex)
        {
            int lPrevRealIndex = lRealIndex - 1;
            if (lPrevRealIndex < 0)
                lPrevRealIndex += capacity;

            Collection[lRealIndex] = Collection[lPrevRealIndex];

            lRealIndex = lPrevRealIndex;
            lastSimulatedIndex--;
        }

        T0 prev = Collection[realIndex];
        Collection[realIndex] = data;
        // If written was not maxed out then increase it.
        if (written < Capacity)
            IncreaseWritten();

        return prev;
    }

    /// <summary>
    /// Adds an entry to the collection, returning a replaced entry.
    /// </summary>
    /// <param name = "data"> Data to add. </param>
    /// <returns> Replaced entry. Value will be default if no entry was replaced. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T0 Add(T0 data)
    {
        T0 current = Collection[WriteIndex];

        Collection[WriteIndex] = data;
        IncreaseWritten();

        return current;
    }

    /// <summary>
    /// Returns the first entry and removes it from the buffer.
    /// </summary>
    /// <returns> The first entry in the buffer, or the default value if the buffer is empty. </returns>
    public T0 Dequeue()
    {
        if (_written == 0)
            return default;

        int capacity = Capacity;
        int offset = capacity - _written + WriteIndex;
        if (offset >= capacity)
            offset -= capacity;

        T0 result = Collection[offset];

        RemoveRange(fromStart: true, 1);

        return result;
    }

    /// <summary>
    /// Returns whether an entry was dequeued and removes it from the buffer if so.
    /// </summary>
    /// <returns> True if an entry was dequeued; otherwise, false. </returns>
    public bool TryDequeue(out T0 result)
    {
        if (_written == 0)
        {
            result = default;

            return false;
        }

        int capacity = Capacity;
        int offset = capacity - _written + WriteIndex;
        if (offset >= capacity)
            offset -= capacity;

        result = Collection[offset];

        RemoveRange(fromStart: true, 1);

        return true;
    }

    /// <summary>
    /// Adds an entry to the collection, returning a replaced entry.
    /// This method internally redirects to Add.
    /// </summary>
    public T0 Enqueue(T0 data) => Add(data);

    /// <summary>
    /// Returns the value at the actual index as it relates to the simulated index.
    /// </summary>
    /// <param name = "simulatedIndex"> Simulated index to return. A value of 0 would return the first simulated index in the collection. </param>
    /// <returns> The value stored at the simulated index. </returns>
    public T0 this[int simulatedIndex]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            int offset = GetRealIndex(simulatedIndex);
            if (offset < 0)
                return default;

            return Collection[offset];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            int offset = GetRealIndex(simulatedIndex);
            if (offset < 0)
                return;

            Collection[offset] = value;
        }
    }

    /// <summary>
    /// Increases written count and handles offset changes.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void IncreaseWritten()
    {
        int capacity = Capacity;

        int writeIndex = WriteIndex + 1;
        if (writeIndex >= capacity)
            writeIndex = 0;
        WriteIndex = writeIndex;

        if (_written < capacity)
            _written++;
    }

    /// <summary>
    /// Outputs the underlying state needed to walk the ring without going through the indexer. Hoisting these in one call avoids repeated property reads when the caller iterates the buffer in a tight loop.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RingBufferWalkState<T0> GetWalkState()
    {
        T0[] collection = Collection;
        int count = _written;
        int capacity = Capacity;

        int baseReal = capacity - count + WriteIndex;
        if (baseReal >= capacity)
            baseReal -= capacity;

        int lastReal = baseReal + count - 1;
        if (lastReal >= capacity)
            lastReal -= capacity;

        return new RingBufferWalkState<T0>(collection, count, capacity, baseReal, lastReal);
    }

    /// <summary>
    /// Returns the real index of the collection using a simulated index.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetRealIndex(int simulatedIndex)
    {
        int written = _written;
        int capacity = Capacity;

        if ((uint)simulatedIndex >= (uint)written)
        {
            if (Logger<RingBuffer<T0>>.IsErrorEnabled)
                Logger<RingBuffer<T0>>.LogError($"Index [{simulatedIndex}] is out of range. Collection Count [{written}] Capacity [{capacity}].");

            return -1;
        }

        int offset = capacity - written + simulatedIndex + WriteIndex;
        if (offset >= capacity)
            offset -= capacity;

        return offset;
    }

    /// <summary>
    /// Removes values from the simulated start of the collection.
    /// </summary>
    /// <param name = "fromStart"> True to remove from the start, false to remove from the end. </param>
    /// <param name = "length"> Number of entries to remove. </param>
    public void RemoveRange(bool fromStart, int length)
    {
        if (length == 0)
            return;

        if (length < 0)
        {
            Logger<RingBuffer<T0>>.LogError("Negative values cannot be removed.");
            return;
        }

        // Full reset if value is at or more than written.
        if (length >= _written)
        {
            Clear();
            return;
        }

        bool isReferenceOrContainsReferences = Polyfill.IsReferenceOrContainsReferences<T0>();
        int capacity = Capacity;

        if (fromStart)
        {
            if (isReferenceOrContainsReferences)
            {
                int startReal = capacity - _written + WriteIndex;
                if (startReal >= capacity)
                    startReal -= capacity;

                ClearCircularRange(startReal, length);
            }

            _written -= length;
        }
        else
        {
            int newWriteIndex = WriteIndex - length;
            if (newWriteIndex < 0)
                newWriteIndex += capacity;

            if (isReferenceOrContainsReferences)
                ClearCircularRange(newWriteIndex, length);

            _written -= length;
            WriteIndex = newWriteIndex;
        }
    }

    /// <summary>
    /// Clears a contiguous range of the underlying collection, wrapping around the end when needed.
    /// </summary>
    /// <param name = "startReal"> The real index at which to begin clearing. </param>
    /// <param name = "length"> The number of slots to clear. </param>
    private void ClearCircularRange(int startReal, int length)
    {
        int capacity = Capacity;
        int firstChunk = capacity - startReal;
        if (firstChunk >= length)
        {
            Array.Clear(Collection, startReal, length);
        }
        else
        {
            Array.Clear(Collection, startReal, firstChunk);
            Array.Clear(Collection, 0, length - firstChunk);
        }
    }

    /// <summary>
    /// Returns an enumerator for the collection.
    /// </summary>
    /// <returns> An enumerator for the collection. </returns>
    public Enumerator GetEnumerator()
    {
        _enumerator.Initialize(this);

        return _enumerator;
    }

    IEnumerator<T0> IEnumerable<T0>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}