using System.Numerics;
using CodeBoost.Types;
using Xunit;

namespace CodeBoost.Tests;

public class VectorRangeTests
{
    // ── Vector2Range ─────────────────────────────────────────────────────

    [Fact]
    public void Vector2Range_VectorConstructor_StoresAxes()
    {
        Vector2Range vector2Range = new(new Vector2(0f, -1f), new Vector2(10f, 1f));

        Assert.Equal(0f, vector2Range.X.Minimum);
        Assert.Equal(10f, vector2Range.X.Maximum);
        Assert.Equal(-1f, vector2Range.Y.Minimum);
        Assert.Equal(1f, vector2Range.Y.Maximum);
    }

    [Fact]
    public void Vector2Range_ScalarConstructor_AppliesToBothAxes()
    {
        Vector2Range vector2Range = new(2f, 6f);

        Assert.Equal(2f, vector2Range.X.Minimum);
        Assert.Equal(6f, vector2Range.X.Maximum);
        Assert.Equal(2f, vector2Range.Y.Minimum);
        Assert.Equal(6f, vector2Range.Y.Maximum);
    }

    [Fact]
    public void Vector2Range_RandomInclusive_StaysInBounds()
    {
        Vector2Range vector2Range = new(new Vector2(0f, -1f), new Vector2(10f, 1f));

        for (int i = 0; i < 200; i++)
        {
            Vector2 sample = vector2Range.RandomInclusive();
            Assert.InRange(sample.X, 0f, 10f);
            Assert.InRange(sample.Y, -1f, 1f);
        }
    }

    // ── Vector3Range ─────────────────────────────────────────────────────

    [Fact]
    public void Vector3Range_VectorConstructor_StoresAxes()
    {
        Vector3Range vector3Range = new(new Vector3(0f, -1f, 5f), new Vector3(10f, 1f, 15f));

        Assert.Equal(0f, vector3Range.X.Minimum);
        Assert.Equal(10f, vector3Range.X.Maximum);
        Assert.Equal(-1f, vector3Range.Y.Minimum);
        Assert.Equal(1f, vector3Range.Y.Maximum);
        Assert.Equal(5f, vector3Range.Z.Minimum);
        Assert.Equal(15f, vector3Range.Z.Maximum);
    }

    [Fact]
    public void Vector3Range_ScalarConstructor_AppliesToAllAxes()
    {
        Vector3Range vector3Range = new(-1f, 1f);

        Assert.Equal(-1f, vector3Range.X.Minimum);
        Assert.Equal(1f, vector3Range.X.Maximum);
        Assert.Equal(-1f, vector3Range.Y.Minimum);
        Assert.Equal(1f, vector3Range.Y.Maximum);
        Assert.Equal(-1f, vector3Range.Z.Minimum);
        Assert.Equal(1f, vector3Range.Z.Maximum);
    }

    [Fact]
    public void Vector3Range_RandomInclusive_StaysInBounds()
    {
        Vector3Range vector3Range = new(new Vector3(0f, -1f, 5f), new Vector3(10f, 1f, 15f));

        for (int i = 0; i < 200; i++)
        {
            Vector3 sample = vector3Range.RandomInclusive();
            Assert.InRange(sample.X, 0f, 10f);
            Assert.InRange(sample.Y, -1f, 1f);
            Assert.InRange(sample.Z, 5f, 15f);
        }
    }
}
