using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CodeBoost.CodeAnalysis;
using CodeBoost.Logging;

namespace CodeBoost.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Returns the enum type name and value as a string.
    /// </summary>
    /// <example>MyEnum.Two</example>
    [PreserveLogic]
    public static string ToTypeNameAndValueString<T0>(this T0 enumValue) where T0 : Enum
    {
        return $"{typeof(T0).Name}.{enumValue}";
    }

    /// <summary>
    /// Returns the enum full type name and value as a string.
    /// </summary>
    /// <example>Namespace.MyEnum.Two</example>
    [PreserveLogic]
    public static string ToTypeFullNameAndValueString<T0>(this T0 enumValue) where T0 : Enum
    {
        return $"{typeof(T0).FullName}.{enumValue}";
    }

    /// <summary>
    /// Returns the enum name and value as a string.
    /// </summary>
    /// <example>MyEnum.Two</example>
    [PreserveLogic]
    public static string ToTypeAndValueString<T0>(this T0 enumValue, bool useFullName) where T0 : Enum
    {
        Type type = typeof(T0);

        string? name = useFullName ? type.FullName : type.Name;
        return $"{name}.{enumValue}";
    }

    /// <summary>
    /// Gets the lowest and highest values for an enum of the underlying type.
    /// </summary>
    /// <remarks>Returns true if the values were able to be retrieved.</remarks>
    public static bool TryGetMinimumAndMaximumSignedValues<T0>(out long minimumValue, out long maximumValue) where T0 : Enum
    {
        if (!TryValidateUnderlyingType<T0>(typeof(long)))
        {
            minimumValue = 0;
            maximumValue = 0;

            return false;
        }

        /* Beyond here we can return true since the
         * enum type was validated. */

        T0[] array = GetValuesAllocated<T0>();

        if (array.Length == 0)
        {
            minimumValue = 0;
            maximumValue = 0;

            return true;
        }

        minimumValue = long.MaxValue;
        maximumValue = long.MinValue;

        for (int i = 0; i < array.Length; i++)
        {
            long value = (long)(object)array[i];
            if (value > maximumValue)
                maximumValue = value;
            else if (value < minimumValue)
                minimumValue = value;
        }

        return true;
    }

    /// <summary>
    /// Gets the lowest and highest values for an enum of the underlying type.
    /// </summary>
    /// <remarks>Returns true if the values were able to be retrieved.</remarks>
    public static bool TryGetMinimumAndMaximumUnsignedValues<T0>(out ulong minimumValue, out ulong maximumValue) where T0 : Enum
    {
        if (!TryValidateUnderlyingType<T0>(typeof(long)))
        {
            minimumValue = 0;
            maximumValue = 0;

            return false;
        }

        /* Beyond here we can return true since the
         * enum type was validated. */

        T0[] array = GetValuesAllocated<T0>();

        if (array.Length == 0)
        {
            minimumValue = 0;
            maximumValue = 0;

            return true;
        }

        minimumValue = ulong.MaxValue;
        maximumValue = ulong.MinValue;

        for (int i = 0; i < array.Length; i++)
        {
            // Unbox to the validated underlying type (long) before converting; unboxing a long-backed enum directly to ulong throws InvalidCastException.
            ulong value = (ulong)(long)(object)array[i];
            if (value > maximumValue)
                maximumValue = value;
            else if (value < minimumValue)
                minimumValue = value;
        }

        return true;
    }
        
    /// <summary>
    /// Gets all values for an enum.
    /// </summary>
    public static T0[] GetValuesAllocated<T0>() where T0 : Enum
    {
        /* Optimized over LINQ, and compatible
         * with lower .NET 2+. */
        return (T0[])Enum.GetValues(typeof(T0));
    }

    /// <summary>
    /// Gets all values for an enum in ascending numeric order.
    /// </summary>
    public static T0[] GetValuesAscendingAllocated<T0>() where T0 : Enum
    {
        T0[] values = GetValuesAllocated<T0>();
        Array.Sort(values, Comparer<T0>.Default);

        return values;
    }

    /// <summary>
    /// Gets all values for an enum in descending numeric order.
    /// </summary>
    public static T0[] GetValuesDescendingAllocated<T0>() where T0 : Enum
    {
        T0[] values = GetValuesAscendingAllocated<T0>();
        Array.Reverse(values);

        return values;
    }

    /// <summary>
    /// Returns if the value contains any of the provided flags without safety checks.
    /// </summary>
    /// <remarks>The type comparison is not checked.</remarks>
    public static int GetValuesCount<T0>() where T0 : Enum => Enum.GetValues(typeof(T0)).Length;

    /// <summary>
    /// Gets the underlying type for an Enum.
    /// </summary>
    public static Type GetUnderlyingType<T0>() where T0 : Enum => Enum.GetUnderlyingType(typeof(T0));

    /// <summary>
    /// Checks if the underlying type is of expected value.
    /// </summary>
    public static bool TryValidateUnderlyingType<T0>(Type expectedValue) where T0 : Enum
    {
        Type underlyingType = Enum.GetUnderlyingType(typeof(T0));

        if (underlyingType != expectedValue)
        {
            typeof(T0).TryGetFullName(out string fullName);
            Logger<Enum>.LogError($"Enum [{fullName}] has an underlying type [{underlyingType.Name}] when [{expectedValue.Name}] is expected.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns if the value contains any of the provided flags without safety checks.
    /// </summary>
    /// <remarks>The type comparison is not checked.</remarks>
    public static bool HasAnyFlagAllocated<T0>(this T0 thisValue, T0 flagsToCheck) where T0 : Enum
    {
        ulong thisValueAsUint64 = Convert.ToUInt64(thisValue);
        ulong flagsToCheckAsUint64 = Convert.ToUInt64(flagsToCheck);

        return (thisValueAsUint64 & flagsToCheckAsUint64) != 0;
    }

    /// <summary>
    /// Returns if the value contains any of the provided flags without safety checks.
    /// </summary>
    /// <remarks>The type comparison is not checked.</remarks>
    public static bool HasAnyFlagUnsafe<T0>(this T0 thisValue, T0 flagsToCheck) where T0 : Enum
    {
        if (!thisValue.ToUInt64Unsafe(out ulong valueAsUInt64))
            return false;
        if (!flagsToCheck.ToUInt64Unsafe(out ulong flagsAsUInt64))
            return false;

        return (valueAsUInt64 & flagsAsUInt64) != 0;
    }

    /// <summary>
    /// Converts a value to a UInt64 using an approach optimized over Convert.
    /// </summary>
    public static bool ToUInt64Unsafe<T0>(this T0 thisValue, out ulong result) where T0 : Enum
    {
        int size = Unsafe.SizeOf<T0>();

        switch (size)
        {
            case 1:
                result = Unsafe.As<T0, byte>(ref thisValue);
                return true;
            case 2:
                result = Unsafe.As<T0, ushort>(ref thisValue);
                return true;
            case 4:
                result = Unsafe.As<T0, uint>(ref thisValue);
                return true;
            case 8:
                result = Unsafe.As<T0, ulong>(ref thisValue);
                return true;
            default:
                result = 0;
                return false;
        }
    }
}