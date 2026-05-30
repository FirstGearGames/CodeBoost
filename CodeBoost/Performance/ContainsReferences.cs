using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeBoost.Performance;

/// <summary>
/// Provides a cached result indicating whether the supplied type is a reference type or transitively contains reference-type fields.
/// </summary>
/// <remarks>
/// Acts as a polyfill for <c>RuntimeHelpers.IsReferenceOrContainsReferences</c> on runtimes that do not expose it.
/// </remarks>
public static class ContainsReferences<T0>
{
    /// <summary>
    /// True when <typeparamref name="T0"/> is a reference type or any field reachable from its layout is a reference type.
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType
    public static readonly bool Value = Evaluate(typeof(T0), []);

    private static bool Evaluate(Type type, HashSet<Type> visited)
    {
        if (!type.IsValueType)
            return true;

        if (type.IsPrimitive || type.IsEnum || type.IsPointer)
            return false;

        if (!visited.Add(type))
            return false;

        FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        for (int i = 0; i < fields.Length; i++)
        {
            if (Evaluate(fields[i].FieldType, visited))
                return true;
        }

        return false;
    }
}
