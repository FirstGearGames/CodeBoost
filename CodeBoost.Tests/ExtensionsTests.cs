using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CodeBoost.Extensions;
using CodeBoost.Mathematics;
using Xunit;

namespace CodeBoost.Tests;

public class ExtensionsTests
{
    // ── ArrayExtensions ──────────────────────────────────────────────────

    [Fact]
    public void ArrayShuffle_PreservesElements()
    {
        int[] array = Enumerable.Range(0, 50).ToArray();
        int[] original = (int[])array.Clone();

        array.Shuffle();

        Assert.Equal(original.Length, array.Length);
        Assert.Equal(original.OrderBy(x => x), array.OrderBy(x => x));
    }

    [Fact]
    public async Task ArrayShuffle_ConcurrentInvocations_DoNotCorrupt()
    {
        const int ThreadCount = 4;
        Task[] tasks = new Task[ThreadCount];

        for (int i = 0; i < ThreadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                int[] localArray = Enumerable.Range(0, 100).ToArray();
                for (int j = 0; j < 50; j++)
                    localArray.Shuffle();

                Assert.Equal(100, localArray.Length);
            });
        }

        await Task.WhenAll(tasks);
    }

    // ── ListExtensions ───────────────────────────────────────────────────

    [Fact]
    public void ListShuffle_PreservesElements()
    {
        List<int> list = Enumerable.Range(0, 50).ToList();
        List<int> original = list.ToList();

        list.Shuffle();

        Assert.Equal(original.Count, list.Count);
        Assert.Equal(original.OrderBy(x => x), list.OrderBy(x => x));
    }

    [Fact]
    public void AddUnique_AddsOnlyOnce()
    {
        List<int> list = new();

        Assert.True(list.AddUnique(1));
        Assert.True(list.AddUnique(2));
        Assert.False(list.AddUnique(1));

        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void RemoveAndReturnFirst_Empty_ReturnsDefault()
    {
        List<int> list = new();

        Assert.Equal(0, list.RemoveAndReturnFirst());
    }

    [Fact]
    public void RemoveAndReturnFirst_NonEmpty_RemovesFirst()
    {
        List<int> list = [1, 2, 3];

        Assert.Equal(1, list.RemoveAndReturnFirst());
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list[0]);
    }

    [Fact]
    public void GetAndRemoveLastValue_Empty_ReturnsDefault()
    {
        List<int> list = new();

        Assert.Equal(0, list.GetAndRemoveLastValue());
    }

    [Fact]
    public void GetAndRemoveLastValue_NonEmpty_RemovesLast()
    {
        List<int> list = [1, 2, 3];

        Assert.Equal(3, list.GetAndRemoveLastValue());
        Assert.Equal(2, list.Count);
    }

    // ── DictionaryExtensions — fixed bug ─────────────────────────────────

    [Fact]
    public void KeysToList_Parameterless_ReturnsKeysNotValues()
    {
        Dictionary<int, string> dict = new()
        {
            { 1, "one" },
            { 2, "two" },
            { 3, "three" },
        };

        List<int> keys = dict.KeysToList();

        Assert.Equal(3, keys.Count);
        Assert.Contains(1, keys);
        Assert.Contains(2, keys);
        Assert.Contains(3, keys);
    }

    [Fact]
    public void KeysToList_RefOverload_PopulatesKeys()
    {
        Dictionary<int, string> dict = new()
        {
            { 1, "one" },
            { 2, "two" },
        };

        List<int> result = new() { 99 };
        dict.KeysToList(ref result);

        Assert.Equal(2, result.Count);
        Assert.Contains(1, result);
        Assert.Contains(2, result);
    }

    [Fact]
    public void ValuesToList_Parameterless_ReturnsValues()
    {
        Dictionary<int, string> dict = new()
        {
            { 1, "one" },
            { 2, "two" },
        };

        List<string> values = dict.ValuesToList();

        Assert.Equal(2, values.Count);
        Assert.Contains("one", values);
        Assert.Contains("two", values);
    }

    [Fact]
    public void ValuesToList_RefOverload_PopulatesValues()
    {
        Dictionary<int, string> dict = new()
        {
            { 1, "one" },
        };

        List<string> result = new() { "stale" };
        dict.ValuesToList(ref result);

        Assert.Single(result);
        Assert.Contains("one", result);
    }

    // ── IEnumerableExtensions ────────────────────────────────────────────

    [Fact]
    public void EnumerableToString_NullSource_ReturnsEmpty()
    {
        IEnumerable<int>? values = null;

        Assert.Equal(string.Empty, EnumerableExtensions.ToString(values!, ", "));
    }

    [Fact]
    public void EnumerableToString_DefaultsToCommaDelimiter()
    {
        int[] values = [1, 2, 3];

        Assert.Equal("1, 2, 3", EnumerableExtensions.ToString<int>(values, ", "));
    }

    [Fact]
    public void EnumerableToString_CustomDelimiter_Joins()
    {
        string[] values = ["a", "b", "c"];

        Assert.Equal("a|b|c", EnumerableExtensions.ToString<string>(values, "|"));
    }

    [Fact]
    public void EnumerableToString_SingleItem_NoTrailingDelimiter()
    {
        int[] values = [42];

        Assert.Equal("42", EnumerableExtensions.ToString<int>(values, ", "));
    }

    // ── BooleanExtensions ────────────────────────────────────────────────

    [Theory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void BooleanToInt_ReturnsExpected(bool value, int expected)
    {
        Assert.Equal(expected, value.ToInt());
    }

    // ── Int32Extensions ──────────────────────────────────────────────────

    [Theory]
    [InlineData(0, 0u)]
    [InlineData(1, 2u)]
    [InlineData(-1, 1u)]
    [InlineData(2, 4u)]
    [InlineData(-2, 3u)]
    public void Int32ToUInt32_ZigZagEncoding(int value, uint expected)
    {
        Assert.Equal(expected, value.ToUInt32());
    }

    [Theory]
    [InlineData(0b_1010, 0b_0010, true)]
    [InlineData(0b_1010, 0b_0001, false)]
    [InlineData(0b_1111, 0b_1010, true)]
    public void Int32FastContains_ChecksFlags(int whole, int part, bool isExpected)
    {
        Assert.Equal(isExpected, whole.FastContains(part));
    }

    [Fact]
    public void Int32AreValuesMatching_AllSame_True()
    {
        int[] values = [1, 1, 1, 1];

        Assert.True(values.AreValuesMatching());
    }

    [Fact]
    public void Int32AreValuesMatching_OneDifferent_False()
    {
        int[] values = [1, 1, 2, 1];

        Assert.False(values.AreValuesMatching());
    }

    [Fact]
    public void Int32AreValuesMatching_NullOrEmpty_True()
    {
        int[]? nullArray = null;
        int[] empty = [];

        Assert.True(nullArray!.AreValuesMatching());
        Assert.True(empty.AreValuesMatching());
    }

    // ── UInt32Extensions ─────────────────────────────────────────────────

    [Theory]
    [InlineData(10u, 3u, RoundingType.Down, 3u)]
    [InlineData(10u, 3u, RoundingType.Up, 4u)]
    [InlineData(0u, 5u, RoundingType.Up, 0u)]
    [InlineData(0u, 5u, RoundingType.UpNonZero, 1u)]
    public void UInt32Divide_AppliesRounding(uint value, uint divisor, RoundingType rounding, uint expected)
    {
        Assert.Equal(expected, value.Divide(divisor, rounding));
    }

    [Fact]
    public void UInt32Divide_ZeroDivisor_ReturnsZero()
    {
        Assert.Equal(0u, 10u.Divide(0u, RoundingType.Down));
    }

    // ── UInt64Extensions ─────────────────────────────────────────────────

    [Theory]
    [InlineData(0UL, 0u)]
    [InlineData(7UL, 1u)]
    [InlineData(8UL, 1u)]
    [InlineData(9UL, 2u)]
    public void ToPackedByteCount_ReturnsBytesNeeded(ulong bitCount, uint expected)
    {
        Assert.Equal(expected, bitCount.ToPackedByteCount());
    }

    [Theory]
    [InlineData(123UL, 5, "00123")]
    [InlineData(99UL, 3, "099")]
    public void UInt64Pad_PadsLeadingZeros(ulong value, int padding, string expected)
    {
        Assert.Equal(expected, value.Pad(padding));
    }

    // ── DoubleExtensions ─────────────────────────────────────────────────

    [Theory]
    [InlineData(-0.5d, 0d)]
    [InlineData(0d, 0d)]
    [InlineData(0.5d, 0.5d)]
    [InlineData(1d, 1d)]
    [InlineData(2d, 1d)]
    public void DoubleClamp01_RestrictsToZeroOne(double value, double expected)
    {
        Assert.Equal(expected, value.Clamp01());
    }

    [Theory]
    [InlineData(1d, 1.000001d, true)]
    [InlineData(1d, 1.5d, false)]
    public void DoubleIsApproximately_ChecksTolerance(double a, double b, bool isExpected)
    {
        Assert.Equal(isExpected, a.IsApproximately(b, 0.0001d));
    }

    [Theory]
    [InlineData(5d, 1d)]
    [InlineData(0d, 1d)]
    [InlineData(-3d, -1d)]
    public void DoubleNonZeroSign_ReturnsSign(double value, double expected)
    {
        Assert.Equal(expected, value.NonZeroSign());
    }

    [Theory]
    [InlineData(5d, 0d, 10d, true)]
    [InlineData(0d, 0d, 10d, true)]
    [InlineData(10d, 0d, 10d, true)]
    [InlineData(-1d, 0d, 10d, false)]
    [InlineData(11d, 0d, 10d, false)]
    public void DoubleIsBetweenInclusive_ChecksRange(double value, double minimum, double maximum, bool isExpected)
    {
        Assert.Equal(isExpected, value.IsBetweenInclusive(minimum, maximum));
    }

    // ── SingleExtensions ─────────────────────────────────────────────────

    [Fact]
    public void SingleAreValuesMatching_AllWithinTolerance_True()
    {
        float[] values = [1f, 1.0000001f, 1.0000002f];

        Assert.True(values.AreValuesMatching(0.001f));
    }

    [Fact]
    public void SingleAreValuesMatching_OneOutsideTolerance_False()
    {
        float[] values = [1f, 1f, 5f];

        Assert.False(values.AreValuesMatching(0.001f));
    }

    // ── Vector2Extensions ────────────────────────────────────────────────

    [Fact]
    public void Vector2IsWithinDistance_DefaultTolerance_True()
    {
        Vector2 a = new(0f, 0f);
        Vector2 b = new(0.005f, 0f);

        Assert.True(a.IsWithinDistance(b));
    }

    [Fact]
    public void Vector2IsWithinDistance_LargeDistance_False()
    {
        Vector2 a = new(0f, 0f);
        Vector2 b = new(10f, 10f);

        Assert.False(a.IsWithinDistance(b));
    }

    [Fact]
    public void Vector2IsNan_DetectsNaN()
    {
        Assert.True(new Vector2(float.NaN, 0f).IsNan());
        Assert.True(new Vector2(0f, float.NaN).IsNan());
        Assert.False(new Vector2(0f, 0f).IsNan());
    }

    [Fact]
    public void Vector2AddVector3_SumsXY()
    {
        Vector2 a = new(1f, 2f);
        Vector3 b = new(3f, 4f, 99f);

        Vector2 result = a.AddVector3(b);

        Assert.Equal(4f, result.X);
        Assert.Equal(6f, result.Y);
    }

    // ── Vector3Extensions ────────────────────────────────────────────────

    [Fact]
    public void Vector3IsNan_DetectsNaN()
    {
        Assert.True(new Vector3(float.NaN, 0f, 0f).IsNan());
        Assert.True(new Vector3(0f, 0f, float.NaN).IsNan());
        Assert.False(new Vector3(1f, 2f, 3f).IsNan());
    }

    [Fact]
    public void Vector3IsWithinAccuracy_TightTolerance()
    {
        Vector3 a = new(1f, 2f, 3f);
        Vector3 b = new(1.001f, 2.001f, 3.001f);

        Assert.True(a.IsWithinAccuracy(b, 0.01f));
        Assert.False(a.IsWithinAccuracy(b, 0.0001f));
    }

    [Fact]
    public void Vector3AddVector2_SumsXY_KeepsZ()
    {
        Vector3 a = new(1f, 2f, 5f);
        Vector2 b = new(3f, 4f);

        Vector3 result = a.AddVector2(b);

        Assert.Equal(4f, result.X);
        Assert.Equal(6f, result.Y);
        Assert.Equal(5f, result.Z);
    }

    // ── EnumExtensions ───────────────────────────────────────────────────

    private enum SampleEnum
    {
        A = 0,
        B = 5,
        C = 10,
    }

    [Flags]
    private enum SampleFlags
    {
        None = 0,
        First = 1,
        Second = 2,
        Third = 4,
    }

    [Fact]
    public void GetValuesAllocated_ReturnsAllValues()
    {
        SampleEnum[] values = EnumExtensions.GetValuesAllocated<SampleEnum>();

        Assert.Equal(3, values.Length);
        Assert.Contains(SampleEnum.A, values);
        Assert.Contains(SampleEnum.B, values);
        Assert.Contains(SampleEnum.C, values);
    }

    [Fact]
    public void GetValuesAscendingAllocated_ReturnsSorted()
    {
        SampleEnum[] sorted = EnumExtensions.GetValuesAscendingAllocated<SampleEnum>();

        Assert.Equal(SampleEnum.A, sorted[0]);
        Assert.Equal(SampleEnum.B, sorted[1]);
        Assert.Equal(SampleEnum.C, sorted[2]);
    }

    [Fact]
    public void GetValuesDescendingAllocated_ReturnsReverseSorted()
    {
        SampleEnum[] sorted = EnumExtensions.GetValuesDescendingAllocated<SampleEnum>();

        Assert.Equal(SampleEnum.C, sorted[0]);
        Assert.Equal(SampleEnum.B, sorted[1]);
        Assert.Equal(SampleEnum.A, sorted[2]);
    }

    [Fact]
    public void HasAnyFlagUnsafe_DetectsMatch()
    {
        SampleFlags value = SampleFlags.First | SampleFlags.Second;

        Assert.True(value.HasAnyFlagUnsafe(SampleFlags.First));
        Assert.True(value.HasAnyFlagUnsafe(SampleFlags.Third | SampleFlags.First));
        Assert.False(value.HasAnyFlagUnsafe(SampleFlags.Third));
    }

    [Fact]
    public void ToUInt64Unsafe_ConvertsCorrectly()
    {
        SampleFlags value = SampleFlags.First | SampleFlags.Third;

        bool isConverted = value.ToUInt64Unsafe(out ulong result);

        Assert.True(isConverted);
        Assert.Equal(5UL, result);
    }

    // ── StringExtensions ─────────────────────────────────────────────────

    [Fact]
    public void StringEmptyIfNull_ReturnsEmptyForNull()
    {
        string? value = null;

        Assert.Equal(string.Empty, value.EmptyIfNull());
    }

    [Fact]
    public void StringEmptyIfNull_ReturnsValueForNonNull()
    {
        Assert.Equal("hello", "hello".EmptyIfNull());
    }

    [Theory]
    [InlineData("helloWorld", "HelloWorld")]
    [InlineData("HelloWorld", "HelloWorld")]
    public void MemberToPascalCase_CapitalizesFirstLetter(string input, string expected)
    {
        Assert.Equal(expected, input.MemberToPascalCase());
    }

    [Fact]
    public void StringStableHashUI32_ReturnsSameForEqualStrings()
    {
        Assert.Equal("hello".GetStableHashUi32(), "hello".GetStableHashUi32());
    }

    [Fact]
    public void StringStableHashUI64_ReturnsSameForEqualStrings()
    {
        Assert.Equal("hello".GetStableHashUInt64(), "hello".GetStableHashUInt64());
    }

    [Fact]
    public void StringToBytesNonAllocated_ReturnsEncodedBytes()
    {
        byte[] bytes = "abc".ToBytesNonAllocated(out int bytesWritten);

        Assert.Equal(3, bytesWritten);
        Assert.Equal((byte)'a', bytes[0]);
        Assert.Equal((byte)'b', bytes[1]);
        Assert.Equal((byte)'c', bytes[2]);
    }

    // ── ArraySegmentExtensions ───────────────────────────────────────────

    [Fact]
    public void ArraySegmentEnumerator_WalksRange()
    {
        int[] backing = [10, 20, 30, 40, 50];
        ArraySegment<int> segment = new(backing, 1, 3);

        List<int> collected = new();
        ArraySegmentExtensions.ArraySegmentEnumerator<int> enumerator = ArraySegmentExtensions.GetEnumerator(segment);
        while (enumerator.MoveNext())
            collected.Add(enumerator.Current);

        Assert.Equal(new[] { 20, 30, 40 }, collected);
    }

    // ── TypeExtensions ───────────────────────────────────────────────────

    [Fact]
    public void TryGetFullName_ReturnsTrueForKnownType()
    {
        bool isFound = typeof(int).TryGetFullName(out string fullName);

        Assert.True(isFound);
        Assert.Equal("System.Int32", fullName);
    }
}
