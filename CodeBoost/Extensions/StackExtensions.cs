using System.Collections.Generic;
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8601 // Possible null reference assignment.

namespace CodeBoost.Extensions;

public static class StackExtensions
{
        
    #if NETSTANDARD2_0 || NETSTANDARD1_6
    public static bool TryPop<T0>(this Stack<T0> stack, out T0 result)
    {
        bool isEmpty = stack.Count == 0;
        result = isEmpty ? default : stack.Pop();

        return !isEmpty;
    }
    #endif
        
}