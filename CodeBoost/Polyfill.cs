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

#if NETSTANDARD2_0
    private static class ReferenceCheck<T>
    {
        public static readonly bool Result = Compute(typeof(T));

        private static bool Compute(Type type)
        {
            if (!type.IsValueType)
                return true;

            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (Compute(field.FieldType))
                    return true;
            }

            return false;
        }
    }

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
