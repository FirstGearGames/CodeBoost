using System.Linq;
using CodeBoost.Types;
using Xunit;

namespace CodeBoost.Tests;

public class InterfaceAndAttributeTests
{
    private sealed class OrderableEntry : IOrderable
    {
        public int Order { get; init; }
    }

    private sealed class WeightedEntry : IWeighted
    {
        private readonly float _weight;
        private readonly UIntRange _quantityRange;

        public WeightedEntry(float weight, uint quantityMax)
        {
            _weight = weight;
            _quantityRange = new(1u, quantityMax);
        }

        public float GetWeight() => _weight;

        public UIntRange GetQuantityRange() => _quantityRange;
    }

    [Fact]
    public void IOrderable_OrderIsSortable()
    {
        OrderableEntry[] entries =
        [
            new() { Order = 5 },
            new() { Order = 1 },
            new() { Order = 3 },
        ];

        OrderableEntry[] sorted = entries.OrderBy(orderableEntry => orderableEntry.Order).ToArray();

        Assert.Equal(1, sorted[0].Order);
        Assert.Equal(3, sorted[1].Order);
        Assert.Equal(5, sorted[2].Order);
    }

    [Fact]
    public void IWeighted_GetWeightAndQuantityRange_ReturnsExpectedValues()
    {
        WeightedEntry weightedEntry = new(2.5f, 10u);

        Assert.Equal(2.5f, weightedEntry.GetWeight());

        UIntRange quantityRange = weightedEntry.GetQuantityRange();
        Assert.Equal(1u, quantityRange.Minimum);
        Assert.Equal(10u, quantityRange.Maximum);
    }

    [Fact]
    public void InternalApiAttribute_DefaultDetails_IsEmptyString()
    {
        InternalApiAttribute internalApiAttribute = new();

        Assert.Equal(string.Empty, internalApiAttribute.Details);
    }

    [Fact]
    public void InternalApiAttribute_WithDetails_StoresMessage()
    {
        InternalApiAttribute internalApiAttribute = new("subject to change");

        Assert.Equal("subject to change", internalApiAttribute.Details);
    }

    [Fact]
    public void ServerOnlyAttribute_CanBeInstantiated()
    {
        ServerOnlyAttribute attribute = new();

        Assert.NotNull(attribute);
    }

    [Fact]
    public void ClientOnlyAttribute_CanBeInstantiated()
    {
        ClientOnlyAttribute attribute = new();

        Assert.NotNull(attribute);
    }
}
