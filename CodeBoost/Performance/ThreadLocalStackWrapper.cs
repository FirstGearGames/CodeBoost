using System;
using System.Collections.Generic;

namespace CodeBoost.Performance;

public class ThreadLocalStackWrapper<TObject>
{
    /// <summary>
    /// The stack for the ThreadLocal.
    /// </summary>
    public readonly Stack<TObject> LocalStack = [];
    /// <summary>
    /// The action to invoke when deconstructing.
    /// </summary>
    private readonly Action<Stack<TObject>> _onFinalize;

    public ThreadLocalStackWrapper(Action<Stack<TObject>> onFinalize)
    {
        _onFinalize = onFinalize;
    }

    ~ThreadLocalStackWrapper()
    {
        _onFinalize?.Invoke(LocalStack);
    }
}