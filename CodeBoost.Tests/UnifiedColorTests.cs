using System.Drawing;
using CodeBoost.Types;
using Xunit;

namespace CodeBoost.Tests;

public class UnifiedColorTests
{
    [Fact]
    public void Constructor_ByteComponents_StoresValues()
    {
        UnifiedColor unifiedColor = new((byte)10, (byte)20, (byte)30, (byte)40);

        Assert.Equal((byte)10, unifiedColor.R);
        Assert.Equal((byte)20, unifiedColor.G);
        Assert.Equal((byte)30, unifiedColor.B);
        Assert.Equal((byte)40, unifiedColor.A);
    }

    [Fact]
    public void Constructor_Color_CopiesComponents()
    {
        Color color = Color.FromArgb(255, 128, 64, 32);

        UnifiedColor unifiedColor = new(color);

        Assert.Equal((byte)128, unifiedColor.R);
        Assert.Equal((byte)64, unifiedColor.G);
        Assert.Equal((byte)32, unifiedColor.B);
        Assert.Equal((byte)255, unifiedColor.A);
    }

    [Theory]
    [InlineData(0f, 0)]
    [InlineData(1f, 255)]
    [InlineData(0.5f, 127)]
    [InlineData(-1f, 0)]
    [InlineData(2f, 255)]
    public void Constructor_FloatComponents_ClampsAndConverts(float componentValue, byte expectedByte)
    {
        UnifiedColor unifiedColor = new(componentValue, componentValue, componentValue, componentValue);

        Assert.Equal(expectedByte, unifiedColor.R);
        Assert.Equal(expectedByte, unifiedColor.G);
        Assert.Equal(expectedByte, unifiedColor.B);
        Assert.Equal(expectedByte, unifiedColor.A);
    }

    [Fact]
    public void FloatAccessors_ReturnNormalizedValues()
    {
        UnifiedColor unifiedColor = new((byte)0, (byte)127, (byte)255, (byte)64);

        Assert.Equal(0f, unifiedColor.Rf);
        Assert.Equal(127f / 255f, unifiedColor.Gf);
        Assert.Equal(1f, unifiedColor.Bf);
        Assert.Equal(64f / 255f, unifiedColor.Af);
    }

    [Fact]
    public void GetColor_ReturnsMatchingColor()
    {
        UnifiedColor unifiedColor = new((byte)10, (byte)20, (byte)30, (byte)200);

        Color color = unifiedColor.GetColor();

        Assert.Equal((byte)10, color.R);
        Assert.Equal((byte)20, color.G);
        Assert.Equal((byte)30, color.B);
        Assert.Equal((byte)200, color.A);
    }
}
