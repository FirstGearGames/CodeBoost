using System.Collections.Generic;
using System.Text;

namespace CodeBoost.Performance;

/// <summary>
/// A pool for a type which is not resettable.
/// </summary>
public static class Utf8EncodingPool
{
    /// <summary>
    /// The stack of pooled instances.
    /// </summary>
    private static readonly Stack<UTF8Encoding> Stack = new();
    /// <summary>
    /// The lock that guards concurrent access to <see cref="Stack"/>.
    /// </summary>
    private static readonly object StackLock = new();

    /// <summary>
    /// Returns a value from the stack or creates a new instance when the stack is empty.
    /// </summary>
    /// <returns>A UTF8Encoding instance from the pool, or a newly constructed one if the pool is empty.</returns>
    public static UTF8Encoding Rent()
    {
        lock (StackLock)
        {
            if (Stack.TryPop(out UTF8Encoding result))
                return result;
        }

        return new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
    }

    /// <summary>
    /// Stores an instance of UTF8Encoding and sets the original reference to default.
    /// The method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref UTF8Encoding value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Stores a value to the stack.
    /// </summary>
    /// <param name = "value"> </param>
    public static void Return(UTF8Encoding value)
    {
        if (value is null)
            return;

        lock (StackLock)
        {
            Stack.Push(value);
        }
    }
}