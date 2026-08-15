using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CodeBoost;

internal static class Polyfill
{
    public static int Clamp(int value, int min, int max) =>
#if NETSTANDARD2_0
        value < min ? min : value > max ? max : value;
#else
        Math.Clamp(value, min, max);
#endif

    public static long Clamp(long value, long min, long max) =>
#if NETSTANDARD2_0
        value < min ? min : value > max ? max : value;
#else
        Math.Clamp(value, min, max);
#endif

    public static float Clamp(float value, float min, float max) =>
#if NETSTANDARD2_0
        value < min ? min : value > max ? max : value;
#else
        Math.Clamp(value, min, max);
#endif

    public static double Clamp(double value, double min, double max) =>
#if NETSTANDARD2_0
        value < min ? min : value > max ? max : value;
#else
        Math.Clamp(value, min, max);
#endif

    public static byte Clamp(byte value, byte min, byte max) =>
#if NETSTANDARD2_0
        value < min ? min : value > max ? max : value;
#else
        Math.Clamp(value, min, max);
#endif

    public static bool IsReferenceOrContainsReferences<T>() =>
#if NETSTANDARD2_0
        ReferenceCheck<T>.Result;
#else
        RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#endif

    /// <summary>
    /// Answers <see cref="IsReferenceOrContainsReferences{T}"/> by walking the type, for the targets whose runtime does not expose
    /// <see cref="RuntimeHelpers.IsReferenceOrContainsReferences{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to inspect.</typeparam>
    /// <remarks>
    /// Compiled on every target even though only <c>NETSTANDARD2_0</c> dispatches to it, so it can be exercised by tests: they run on
    /// a target that takes the intrinsic instead, and a fallback that is only compiled where nothing can run it ships unproven. There
    /// is no cost to a target that does not use it, since a generic type's static constructor runs only if something touches it.
    /// </remarks>
    internal static class ReferenceCheck<T>
    {
        /// <summary>
        /// True when <typeparamref name="T"/> is a reference type, or a value type holding one anywhere in its layout.
        /// </summary>
        public static readonly bool Result = Compute(typeof(T), []);

        private static bool Compute(Type type, HashSet<Type> visitedTypes)
        {
            if (!type.IsValueType)
                return true;

            /* A primitive holds a field of its own type: typeof(int) declares m_value as an int, so walking its fields without
             * stopping here recurses on int forever and takes the stack with it. Enums are the same story through their backing
             * field. */
            if (type.IsPrimitive || type.IsEnum)
                return false;

            // A struct can reach itself through a field of a struct that holds it, so a type already being walked answers false.
            if (!visitedTypes.Add(type))
                return false;

            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (Compute(field.FieldType, visitedTypes))
                    return true;
            }

            return false;
        }
    }

#if NETSTANDARD2_0
    public static bool TryPop<T>(this Stack<T> stack, out T result)
    {
        if (stack.Count == 0)
        {
            result = default!;
            return false;
        }

        result = stack.Pop();
        return true;
    }
#endif
}
