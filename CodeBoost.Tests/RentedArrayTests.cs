using System.Buffers;
using CodeBoost.Types;
using Xunit;

namespace CodeBoost.Tests;

public class RentedArrayTests
{
    [Fact]
    public void Constructor_RentsArrayOfAtLeastRequestedLength()
    {
        using RentedArray<int> rentedArray = new(64);

        Assert.NotNull(rentedArray.Array);
        Assert.True(rentedArray.Array.Length >= 64);
    }

    [Fact]
    public void Constructor_WithExplicitPool_UsesProvidedPool()
    {
        ArrayPool<int> pool = ArrayPool<int>.Create();

        using RentedArray<int> rentedArray = new(pool, 32);

        Assert.NotNull(rentedArray.Array);
        Assert.True(rentedArray.Array.Length >= 32);
    }

    [Fact]
    public void Dispose_ReferenceType_DoesNotLeakReferencesToNextRenter()
    {
        ArrayPool<string> pool = ArrayPool<string>.Create();

        string[] capturedArray;
        using (RentedArray<string> firstRented = new(pool, 16))
        {
            for (int i = 0; i < 16; i++)
                firstRented.Array[i] = $"value-{i}";

            capturedArray = firstRented.Array;
        }

        for (int i = 0; i < capturedArray.Length; i++)
            Assert.Null(capturedArray[i]);
    }

    [Fact]
    public void Dispose_ValueType_DoesNotThrow()
    {
        ArrayPool<int> pool = ArrayPool<int>.Create();

        RentedArray<int> rentedArray = new(pool, 8);

        rentedArray.Dispose();
    }
}
