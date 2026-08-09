using System;
using System.Collections.Generic;

namespace CodeBoost.Performance;

public class ThreadLocalStackWrapper<T0>
{
    /// <summary>
    /// The stack for the ThreadLocal.
    /// </summary>
    public readonly Stack<T0> LocalStack = [];
    /// <summary>
    /// The action to invoke when deconstructing.
    /// </summary>
    private readonly Action<Stack<T0>> _onFinalize;

    public ThreadLocalStackWrapper(Action<Stack<T0>> onFinalize)
    {
        _onFinalize = onFinalize;
    }

    ~ThreadLocalStackWrapper()
    {
        _onFinalize?.Invoke(LocalStack);
    }
}