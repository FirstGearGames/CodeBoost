using System;
using System.Runtime.CompilerServices;
using CodeBoost.Logging;
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8601 // Possible null reference assignment.


namespace CodeBoost.Types;

/// <summary>
/// Unity 2022 has a bug where codegen will not compile when referencing a Queue type,
/// while also targeting .Net as the framework API.
/// As a work around this class is used for queues instead.
/// </summary>
public class BoostedQueue<T0>
{
    /// <summary>
    /// The maximum size of the collection.
    /// </summary>
    public int Capacity => _collection.Length;
    /// <summary>
    /// The number of elements in the queue.
    /// </summary>
    public int Count => _written;
    /// <summary>
    /// The collection containing the data.
    /// </summary>
    private T0[] _collection = new T0[4];
    /// <summary>
    /// The current write index of the collection.
    /// </summary>
    public int WriteIndex { get; private set; }
    /// <summary>
    /// The buffer used for resizing.
    /// </summary>
    private readonly T0[] _resizeBuffer = [];
    /// <summary>
    /// The read position of the next dequeue.
    /// </summary>
    private int _read;
    /// <summary>
    /// The length of the queue.
    /// </summary>
    private int _written;

    /// <summary>
    /// Enqueues an entry.
    /// </summary>
    /// <param name = "data"> </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T0 data)
    {
        if (_written == _collection.Length)
            Resize();

        int writeIndex = WriteIndex;
        if (writeIndex >= _collection.Length)
            writeIndex = 0;

        _collection[writeIndex] = data;

        WriteIndex = writeIndex + 1;
        _written++;
    }

    /// <summary>
    /// Tries to dequeue the next entry.
    /// </summary>
    /// <param name = "result"> Dequeued entry. </param>
    /// <param name = "defaultArrayEntry"> True to set the array entry as default. </param>
    /// <returns> True if an entry existed to dequeue. </returns>
    public bool TryDequeue(out T0 result, bool defaultArrayEntry = true)
    {
        if (_written == 0)
        {
            result = default;

            return false;
        }

        result = Dequeue(defaultArrayEntry);

        return true;
    }

    /// <summary>
    /// Dequeues the next entry.
    /// </summary>
    /// <param name = "defaultArrayEntry"> True to set the array entry as default. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T0 Dequeue(bool defaultArrayEntry = true)
    {
        if (_written == 0)
            return default;

        int read = _read;
        T0 result = _collection[read];
        if (defaultArrayEntry)
            _collection[read] = default;

        _written--;

        read++;
        if (read >= _collection.Length)
            read = 0;
        _read = read;

        return result;
    }

    /// <summary>
    /// Tries to peek the next entry.
    /// </summary>
    /// <param name = "result"> Peeked entry. </param>
    /// <returns> True if an entry existed to peek. </returns>
    public bool TryPeek(out T0 result)
    {
        if (_written == 0)
        {
            result = default;

            return false;
        }

        result = Peek();

        return true;
    }

    /// <summary>
    /// Peeks the next queue entry.
    /// </summary>
    /// <returns> The next entry in the queue. </returns>
    public T0 Peek()
    {
        if (_written == 0)
            throw new($"Queue of type {typeof(T0).Name} is empty.");

        return _collection[_read];
    }

    /// <summary>
    /// Returns an entry at index or default if index is invalid.
    /// </summary>
    public T0 GetIndexOrDefault(int simulatedIndex)
    {
        int offset = GetRealIndex(simulatedIndex, log: false);
        if (offset == -1)
            return default;

        return _collection[offset];
    }

    /// <summary>
    /// Clears the queue.
    /// </summary>
    public void Clear()
    {
        _read = 0;
        WriteIndex = 0;
        _written = 0;

        if (_collection.Length > 0)
            Array.Clear(_collection, 0, _collection.Length);
    }

    /// <summary>
    /// Resets the read and write indices without touching the underlying array. Callers that have already nulled their populated slots use this to skip the redundant <see cref="Array.Clear(System.Array, int, int)"/> in <see cref="Clear"/>.
    /// </summary>
    public void ResetWriteState()
    {
        _read = 0;
        WriteIndex = 0;
        _written = 0;
    }

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
            int offset = GetRealIndex(simulatedIndex, log: true);

            return _collection[offset];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            int offset = GetRealIndex(simulatedIndex, log: true);
            _collection[offset] = value;
        }
    }

    /// <summary>
    /// Doubles the queue size.
    /// </summary>
    private void Resize()
    {
        int length = _written;
        int doubleLength = length * 2;
        int read = _read;

        /* Make sure copy array is the same size as current
         * and copy contents into it. */
        // Ensure large enough to fit contents.
        T0[] resizeBuffer = _resizeBuffer;
        if (resizeBuffer.Length < doubleLength)
            Array.Resize(ref resizeBuffer, doubleLength);
        // Copy from the read of queue first.
        int copyLength = length - read;
        Array.Copy(_collection, read, resizeBuffer, 0, copyLength);
        /* If read index was higher than 0
         * then copy remaining data as well from 0. */
        if (read > 0)
            Array.Copy(_collection, 0, resizeBuffer, copyLength, read);

        // Set _array to resize.
        _collection = resizeBuffer;
        // Reset positions.
        _read = 0;
        WriteIndex = length;
    }

    /// <summary>
    /// Returns the real index of the collection using a simulated index.
    /// </summary>
    /// <param name = "simulatedIndex"> Simulated index to translate. </param>
    /// <param name = "log"> True to log when the index is out of range. </param>
    /// <returns> The real index of the collection, or -1 when the simulated index is out of range. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetRealIndex(int simulatedIndex, bool log)
    {
        int written = _written;
        int capacity = _collection.Length;

        if ((uint)simulatedIndex >= (uint)written)
        {
            if (log && Logger<BoostedQueue<T0>>.IsErrorEnabled)
                Logger<BoostedQueue<T0>>.LogError($"Index {simulatedIndex} is out of range. Collection count is {written}, Capacity is {capacity}");

            return -1;
        }

        int offset = capacity - written + simulatedIndex + WriteIndex;
        if (offset >= capacity)
            offset -= capacity;

        return offset;
    }
}
