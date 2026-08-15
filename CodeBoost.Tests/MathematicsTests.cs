using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using CodeBoost.Mathematics;
using Xunit;

namespace CodeBoost.Tests;

public class MathematicsTests
{
    [Fact]
    public void RandomInclusive_Double_StaysInBounds()
    {
        for (int i = 0; i < 500; i++)
        {
            double value = MathCb.RandomInclusive(-5d, 5d);
            Assert.InRange(value, -5d, 5d + double.Epsilon);
        }
    }

    [Fact]
    public void RandomExclusive_Double_StaysBelowMax()
    {
        for (int i = 0; i < 500; i++)
        {
            double value = MathCb.RandomExclusive(0d, 1d);
            Assert.True(value >= 0d);
            Assert.True(value < 1d);
        }
    }

    [Fact]
    public void Random01_StaysInBounds()
    {
        for (int i = 0; i < 500; i++)
        {
            double value = MathCb.Random01();
            Assert.InRange(value, 0d, 1d + double.Epsilon);
        }
    }

    [Theory]
    [InlineData(0d, 0d)]
    [InlineData(0.5d, 5d)]
    [InlineData(1d, 10d)]
    [InlineData(-1d, 0d)]
    [InlineData(2d, 10d)]
    public void Lerp_ClampedAndInterpolated(double percentage, double expected)
    {
        double result = MathCb.Lerp(0d, 10d, percentage);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Min_Long_ReturnsLower()
    {
        Assert.Equal(-3L, MathCb.Min(-3L, 5L));
        Assert.Equal(2L, MathCb.Min(7L, 2L));
    }

    [Fact]
    public void Min_ULong_ReturnsLower()
    {
        Assert.Equal(2UL, MathCb.Min(2UL, 7UL));
    }

    [Fact]
    public void RandomSign_OverManyCalls_ProducesBothSigns()
    {
        bool isPositiveSeen = false;
        bool isNegativeSeen = false;

        for (int i = 0; i < 200; i++)
        {
            double signed = MathCb.RandomSign(5f);

            if (signed > 0)
                isPositiveSeen = true;
            else if (signed < 0)
                isNegativeSeen = true;

            if (isPositiveSeen && isNegativeSeen)
                break;
        }

        Assert.True(isPositiveSeen);
        Assert.True(isNegativeSeen);
    }

    [Fact]
    public async Task RandomInclusive_ConcurrentAccess_DoesNotCorrupt()
    {
        const int ThreadCount = 8;
        const int IterationsPerThread = 1000;

        Task[] tasks = new Task[ThreadCount];
        for (int i = 0; i < ThreadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < IterationsPerThread; j++)
                {
                    double value = MathCb.RandomInclusive(0d, 100d);
                    Assert.InRange(value, 0d, 100d + double.Epsilon);
                }
            });
        }

        await Task.WhenAll(tasks);
    }

    [Fact]
    public void InverseLerp_Vector2_ProducesNormalizedPosition()
    {
        Vector2 a = new(0f, 0f);
        Vector2 b = new(10f, 0f);

        Assert.Equal(0f, MathCb.InverseLerp(a, b, new Vector2(0f, 0f)));
        Assert.Equal(0.5f, MathCb.InverseLerp(a, b, new Vector2(5f, 0f)));
        Assert.Equal(1f, MathCb.InverseLerp(a, b, new Vector2(10f, 0f)));
    }

    [Fact]
    public void InverseLerp_Vector3_ProducesNormalizedPosition()
    {
        Vector3 a = new(0f, 0f, 0f);
        Vector3 b = new(0f, 0f, 4f);

        Assert.Equal(0f, MathCb.InverseLerp(a, b, new Vector3(0f, 0f, 0f)));
        Assert.Equal(0.5f, MathCb.InverseLerp(a, b, new Vector3(0f, 0f, 2f)));
        Assert.Equal(1f, MathCb.InverseLerp(a, b, new Vector3(0f, 0f, 4f)));
    }

    [Fact]
    public void Lerp3_Vector2_ProducesQuadraticBezier()
    {
        Vector2 a = new(0f, 0f);
        Vector2 b = new(5f, 5f);
        Vector2 c = new(10f, 0f);

        Vector2 start = MathCb.Lerp3(a, b, c, 0f);
        Vector2 end = MathCb.Lerp3(a, b, c, 1f);

        Assert.Equal(a, start);
        Assert.Equal(c, end);
    }

    [Fact]
    public void Lerp3_Vector3_ProducesQuadraticBezier()
    {
        Vector3 a = new(0f, 0f, 0f);
        Vector3 b = new(5f, 5f, 0f);
        Vector3 c = new(10f, 0f, 0f);

        Vector3 start = MathCb.Lerp3(a, b, c, 0f);
        Vector3 end = MathCb.Lerp3(a, b, c, 1f);

        Assert.Equal(a, start);
        Assert.Equal(c, end);
    }

    [Fact]
    public void SingleToUInt32Unsafe_ConvertsValue()
    {
        uint converted = MathCb.SingleToUInt32Unsafe(2.5d, 0.01f);

        Assert.True(converted > 0);
    }

    /// <summary>
    /// The conversion round-trips across the range it is defined over, including the extremes of <see cref="int"/> and the rounding
    /// boundary, which is the whole of what the method promises.
    /// </summary>
    /// <remarks>
    /// Nothing here asserts what an out-of-range input does. The method is named Unsafe for exactly that reason: keeping the scaled
    /// value inside <see cref="int"/> is the caller's business, so pinning the platform's conversion of one that is not would be
    /// inventing a contract the method never offered, and would stop it ever being made faster.
    /// </remarks>
    [Theory]
    [InlineData(0d, 1f, 0)]
    [InlineData(2.5d, 0.01f, 250)]
    [InlineData(-2.5d, 0.01f, -250)]
    [InlineData(0.5d, 1f, 1)]
    [InlineData(-0.5d, 1f, -1)]
    [InlineData(int.MaxValue, 1f, int.MaxValue)]
    [InlineData(int.MinValue, 1f, int.MinValue)]
    public void SingleToUInt32Unsafe_InRange_ZigZagsTheScaledValue(double value, float accuracy, int expectedWholeValue)
    {
        // The documented encoding, so the expectation is stated independently of the implementation rather than by calling it.
        uint expected = (uint)((expectedWholeValue << 1) ^ (expectedWholeValue >> 31));

        Assert.Equal(expected, MathCb.SingleToUInt32Unsafe(value, accuracy));
    }
}
