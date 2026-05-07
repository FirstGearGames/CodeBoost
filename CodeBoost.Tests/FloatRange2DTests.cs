using System.Numerics;
using CodeBoost.Types;
using Xunit;

namespace CodeBoost.Tests;

public class FloatRange2DTests
{
    [Fact]
    public void Constructor_FloatRanges_StoresAxes()
    {
        FloatRange xRange = new(0f, 10f);
        FloatRange yRange = new(-5f, 5f);

        FloatRange2D floatRange2D = new(xRange, yRange);

        Assert.Equal(0f, floatRange2D.X.Minimum);
        Assert.Equal(10f, floatRange2D.X.Maximum);
        Assert.Equal(-5f, floatRange2D.Y.Minimum);
        Assert.Equal(5f, floatRange2D.Y.Maximum);
    }

    [Fact]
    public void Constructor_FourFloats_StoresAxes()
    {
        FloatRange2D floatRange2D = new(0f, 10f, -5f, 5f);

        Assert.Equal(0f, floatRange2D.X.Minimum);
        Assert.Equal(10f, floatRange2D.X.Maximum);
        Assert.Equal(-5f, floatRange2D.Y.Minimum);
        Assert.Equal(5f, floatRange2D.Y.Maximum);
    }

    [Fact]
    public void ClampX_RestrictsToXAxis()
    {
        FloatRange2D floatRange2D = new(0f, 10f, -5f, 5f);

        Assert.Equal(0f, floatRange2D.ClampX(-3f));
        Assert.Equal(7f, floatRange2D.ClampX(7f));
        Assert.Equal(10f, floatRange2D.ClampX(20f));
    }

    [Fact]
    public void ClampY_RestrictsToYAxis()
    {
        FloatRange2D floatRange2D = new(0f, 10f, -5f, 5f);

        Assert.Equal(-5f, floatRange2D.ClampY(-99f));
        Assert.Equal(2f, floatRange2D.ClampY(2f));
        Assert.Equal(5f, floatRange2D.ClampY(99f));
    }

    [Fact]
    public void Clamp_Vector2_ClampsBothAxes()
    {
        FloatRange2D floatRange2D = new(0f, 10f, -5f, 5f);

        Vector2 clamped = floatRange2D.Clamp(new Vector2(20f, -99f));

        Assert.Equal(10f, clamped.X);
        Assert.Equal(-5f, clamped.Y);
    }

    [Fact]
    public void Clamp_Vector3_PreservesZ()
    {
        FloatRange2D floatRange2D = new(0f, 10f, -5f, 5f);

        Vector3 clamped = floatRange2D.Clamp(new Vector3(20f, -99f, 42f));

        Assert.Equal(10f, clamped.X);
        Assert.Equal(-5f, clamped.Y);
        Assert.Equal(42f, clamped.Z);
    }
}
