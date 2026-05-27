using System.Collections.Generic;
using System.Threading;

namespace CodeBoost.Performance;

//xml
public static class ResettableT1SortedDictionaryPool<T0, T1>
{
    /// <summary>
    /// The stack for the ThreadLocal Dictionary.
    /// </summary>
    private static readonly ThreadLocal<ThreadLocalStackWrapper<SortedDictionary<T0, T1>>> Wrapper;
    /// <summary>
    /// The stack for the global Dictionary.
    /// </summary>
    private static readonly Stack<SortedDictionary<T0, T1>> GlobalStack = [];
    /// <summary>
    /// Maximum number of entries allowed in the global stack.
    /// </summary>
    private const int MaximumGlobalStackSize = 200;
    /// <summary>
    /// Maximum number of entries allowed in the ThreadLocal stack.
    /// </summary>
    private const int MaximumThreadLocalStackSize = 100;

    static ResettableT1SortedDictionaryPool()
    {
        Wrapper = new(valueFactory: () => new(Flush), trackAllValues: false);
    }

    /// <summary>
    /// Rents a Dictionary from the pool.
    /// </summary>
    /// <returns>A cleared Dictionary collection.</returns>
    public static SortedDictionary<T0, T1> Rent()
    {
        Stack<SortedDictionary<T0, T1>> localStack = Wrapper.Value.LocalStack;
        if (localStack.TryPop(out SortedDictionary<T0, T1> result))
            return result;

        lock (GlobalStack)
        {
            if (GlobalStack.TryPop(out result))
                return result;
        }

        return new();
    }

    /// <summary>
    /// Returns a Dictionary to the pool and sets the provided reference to null.
    /// This method will not execute if the value is null.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void ReturnAndNullifyReference(ref SortedDictionary<T0, T1> value)
    {
        Return(value);

        value = null;
    }

    /// <summary>
    /// Returns a Dictionary to the pool.
    /// </summary>
    /// <param name = "value"> Value to return. </param>
    public static void Return(SortedDictionary<T0, T1> value)
    {
        if (value is null)
            return;

        value.Clear();

        Stack<SortedDictionary<T0, T1>> localStack = Wrapper.Value.LocalStack;
        if (localStack.Count < MaximumThreadLocalStackSize)
        {
            localStack.Push(value);
            return;
        }

        lock (GlobalStack)
        {
            if (GlobalStack.Count < MaximumGlobalStackSize)
                GlobalStack.Push(value);
        }

        //If here both stacks are at capacity.
    }

    /// <summary>
    /// Flushes the ThreadLocal Dictionary stack into the global stack.
    /// </summary>
    private static void Flush(Stack<SortedDictionary<T0, T1>> localStack)
    {
        if (localStack.Count == 0)
            return;

        lock (GlobalStack)
        {
            while (localStack.TryPop(out SortedDictionary<T0, T1> item))
            {
                if (GlobalStack.Count < MaximumGlobalStackSize)
                    GlobalStack.Push(item);
                else
                    break;
            }
        }
    }
}