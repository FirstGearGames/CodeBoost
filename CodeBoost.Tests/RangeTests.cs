using CodeBoost.Types;
using Xunit;

namespace CodeBoost.Tests;

public class RangeTests
{
    // ── FloatRange ───────────────────────────────────────────────────────

    [Fact]
    public void FloatRange_Constructor_StoresMinMax()
    {
        FloatRange floatRange = new(1.5f, 9.5f);

        Assert.Equal(1.5f, floatRange.Minimum);
        Assert.Equal(9.5f, floatRange.Maximum);
    }

    [Fact]
    public void FloatRange_RandomInclusive_StaysInBounds()
    {
        FloatRange floatRange = new(2f, 5f);

        for (int i = 0; i < 200; i++)
        {
            float value = floatRange.RandomInclusive();
            Assert.InRange(value, 2f, 5f);
        }
    }

    [Theory]
    [InlineData(0f, 0f)]
    [InlineData(0.5f, 5f)]
    [InlineData(1f, 10f)]
    public void FloatRange_Lerp_ReturnsExpectedValue(float percentage, float expected)
    {
        FloatRange floatRange = new(0f, 10f);

        Assert.Equal(expected, floatRange.Lerp(percentage));
    }

    // ── IntRange ─────────────────────────────────────────────────────────

    [Fact]
    public void IntRange_Constructor_StoresMinMax()
    {
        IntRange intRange = new(-3, 7);

        Assert.Equal(-3, intRange.Minimum);
        Assert.Equal(7, intRange.Maximum);
    }

    [Fact]
    public void IntRange_RandomInclusive_StaysInBounds()
    {
        IntRange intRange = new(0, 10);

        for (int i = 0; i < 200; i++)
        {
            int value = intRange.RandomInclusive();
            Assert.InRange(value, 0, 10);
        }
    }

    [Fact]
    public void IntRange_RandomExclusive_StaysBelowMax()
    {
        IntRange intRange = new(0, 5);

        for (int i = 0; i < 200; i++)
        {
            int value = intRange.RandomExclusive();
            Assert.InRange(value, 0, 4);
        }
    }

    // ── UIntRange ────────────────────────────────────────────────────────

    [Fact]
    public void UIntRange_Constructor_StoresMinMax()
    {
        UIntRange uintRange = new(2u, 8u);

        Assert.Equal(2u, uintRange.Minimum);
        Assert.Equal(8u, uintRange.Maximum);
    }

    [Fact]
    public void UIntRange_RandomInclusive_StaysInBounds()
    {
        UIntRange uintRange = new(1u, 10u);

        for (int i = 0; i < 200; i++)
        {
            uint value = uintRange.RandomInclusive();
            Assert.InRange(value, 1u, 10u);
        }
    }

    [Theory]
    [InlineData(0u, false)]
    [InlineData(2u, true)]
    [InlineData(5u, true)]
    [InlineData(10u, true)]
    [InlineData(11u, false)]
    public void UIntRange_InRange_ReturnsExpected(uint value, bool isExpected)
    {
        UIntRange uintRange = new(2u, 10u);

        Assert.Equal(isExpected, uintRange.InRange(value));
    }

    // ── ByteRange ────────────────────────────────────────────────────────

    [Fact]
    public void ByteRange_Constructor_StoresMinMax()
    {
        ByteRange byteRange = new(20, 200);

        Assert.Equal((byte)20, byteRange.Minimum);
        Assert.Equal((byte)200, byteRange.Maximum);
    }

    [Fact]
    public void ByteRange_RandomInclusive_StaysInBounds()
    {
        ByteRange byteRange = new(50, 100);

        for (int i = 0; i < 200; i++)
        {
            byte value = byteRange.RandomInclusive();
            Assert.InRange(value, (byte)50, (byte)100);
        }
    }

    [Theory]
    [InlineData(0, 50)]
    [InlineData(75, 75)]
    [InlineData(255, 100)]
    public void ByteRange_Clamp_RestrictsValue(byte input, byte expected)
    {
        ByteRange byteRange = new(50, 100);

        Assert.Equal(expected, byteRange.Clamp(input));
    }

    [Theory]
    [InlineData(49, false)]
    [InlineData(50, true)]
    [InlineData(75, true)]
    [InlineData(100, true)]
    [InlineData(101, false)]
    public void ByteRange_InRange_ReturnsExpected(byte value, bool isExpected)
    {
        ByteRange byteRange = new(50, 100);

        Assert.Equal(isExpected, byteRange.InRange(value));
    }
}
