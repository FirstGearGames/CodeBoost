using System;
using System.Numerics;
using CodeBoost.Types;
using Xunit;

namespace CodeBoost.Tests;

public class VectorIntTests
{
    // ── Vector2Int ───────────────────────────────────────────────────────

    [Fact]
    public void Vector2Int_Default_IsZero()
    {
        Vector2Int vector2Int = new();

        Assert.Equal(0, vector2Int.X);
        Assert.Equal(0, vector2Int.Y);
    }

    [Fact]
    public void Vector2Int_ExplicitComponents_StoresValues()
    {
        Vector2Int vector2Int = new(7, -3);

        Assert.Equal(7, vector2Int.X);
        Assert.Equal(-3, vector2Int.Y);
    }

    [Fact]
    public void Vector2Int_CopyConstructor_CopiesComponents()
    {
        Vector2Int source = new(2, 4);

        Vector2Int copy = new(source);

        Assert.Equal(source.X, copy.X);
        Assert.Equal(source.Y, copy.Y);
    }

    [Theory]
    [InlineData(1.4f, 2.6f, 1, 3)]
    [InlineData(-1.6f, 5.4f, -2, 5)]
    public void Vector2Int_Vector2Constructor_RoundsComponents(float x, float y, int expectedX, int expectedY)
    {
        Vector2Int vector2Int = new(new Vector2(x, y), MidpointRounding.AwayFromZero);

        Assert.Equal(expectedX, vector2Int.X);
        Assert.Equal(expectedY, vector2Int.Y);
    }

    // ── Vector3Int ───────────────────────────────────────────────────────

    [Fact]
    public void Vector3Int_Default_IsZero()
    {
        Vector3Int vector3Int = new();

        Assert.Equal(0, vector3Int.X);
        Assert.Equal(0, vector3Int.Y);
        Assert.Equal(0, vector3Int.Z);
    }

    [Fact]
    public void Vector3Int_ExplicitComponents_StoresValues()
    {
        Vector3Int vector3Int = new(1, 2, 3);

        Assert.Equal(1, vector3Int.X);
        Assert.Equal(2, vector3Int.Y);
        Assert.Equal(3, vector3Int.Z);
    }

    [Fact]
    public void Vector3Int_CopyConstructor_CopiesComponents()
    {
        Vector3Int source = new(5, 6, 7);

        Vector3Int copy = new(source);

        Assert.Equal(source.X, copy.X);
        Assert.Equal(source.Y, copy.Y);
        Assert.Equal(source.Z, copy.Z);
    }

    [Fact]
    public void Vector3Int_Vector3Constructor_RoundsComponents()
    {
        Vector3Int vector3Int = new(new Vector3(1.6f, -1.4f, 2.5f), MidpointRounding.AwayFromZero);

        Assert.Equal(2, vector3Int.X);
        Assert.Equal(-1, vector3Int.Y);
        Assert.Equal(3, vector3Int.Z);
    }

    // ── Vector4Int ───────────────────────────────────────────────────────

    [Fact]
    public void Vector4Int_Default_IsZero()
    {
        Vector4Int vector4Int = new();

        Assert.Equal(0, vector4Int.X);
        Assert.Equal(0, vector4Int.Y);
        Assert.Equal(0, vector4Int.Z);
        Assert.Equal(0, vector4Int.W);
    }

    [Fact]
    public void Vector4Int_ExplicitComponents_StoresValues()
    {
        Vector4Int vector4Int = new(1, 2, 3, 4);

        Assert.Equal(1, vector4Int.X);
        Assert.Equal(2, vector4Int.Y);
        Assert.Equal(3, vector4Int.Z);
        Assert.Equal(4, vector4Int.W);
    }

    [Fact]
    public void Vector4Int_CopyConstructor_CopiesComponents()
    {
        Vector4Int source = new(1, 2, 3, 4);

        Vector4Int copy = new(source);

        Assert.Equal(source.X, copy.X);
        Assert.Equal(source.Y, copy.Y);
        Assert.Equal(source.Z, copy.Z);
        Assert.Equal(source.W, copy.W);
    }

    [Fact]
    public void Vector4Int_Vector4Constructor_RoundsComponents()
    {
        Vector4Int vector4Int = new(new Vector4(1.6f, -1.4f, 2.5f, -3.5f), MidpointRounding.AwayFromZero);

        Assert.Equal(2, vector4Int.X);
        Assert.Equal(-1, vector4Int.Y);
        Assert.Equal(3, vector4Int.Z);
        Assert.Equal(-4, vector4Int.W);
    }
}
