using System.Collections.Generic;
using CodeBoost.Types;
using Xunit;

namespace CodeBoost.Tests;

public class WeightedTests
{
    private sealed class WeightedItem : IWeighted
    {
        public string Name { get; }

        public WeightedItem(string name, float weight, uint quantityMax)
        {
            Name = name;
            _weight = weight;
            _quantityRange = new(1u, quantityMax);
        }

        private readonly float _weight;
        private readonly UIntRange _quantityRange;

        public float GetWeight() => _weight;

        public UIntRange GetQuantityRange() => _quantityRange;
    }

    [Fact]
    public void GetValues_NullSource_LogsAndReturns()
    {
        Dictionary<WeightedItem, uint> results = new();

        Weighted.GetValues(null!, 1u, ref results);

        Assert.Empty(results);
    }

    [Fact]
    public void GetValues_EmptySource_NoResults()
    {
        List<WeightedItem> source = new();
        Dictionary<WeightedItem, uint> results = new();

        Weighted.GetValues(source, 5u, ref results);

        Assert.Empty(results);
    }

    [Fact]
    public void GetValues_ZeroCount_NoResults()
    {
        List<WeightedItem> source =
        [
            new("a", 1f, 10u),
            new("b", 1f, 10u),
        ];

        Dictionary<WeightedItem, uint> results = new();

        Weighted.GetValues(source, 0u, ref results);

        Assert.Empty(results);
    }

    [Fact]
    public void GetValues_SingleEntry_AlwaysSelected()
    {
        WeightedItem only = new("only", 1f, 1u);
        List<WeightedItem> source = [only];
        Dictionary<WeightedItem, uint> results = new();

        Weighted.GetValues(source, 1u, ref results);

        Assert.Single(results);
        Assert.True(results.ContainsKey(only));
        Assert.Equal(1u, results[only]);
    }

    [Fact]
    public void GetValues_AllowRepeats_ReachesRequestedCount()
    {
        WeightedItem only = new("only", 1f, 10u);
        List<WeightedItem> source = [only];
        Dictionary<WeightedItem, uint> results = new();

        Weighted.GetValues(source, 3u, ref results, allowRepeatingEntries: true);

        Assert.True(results.ContainsKey(only));
        Assert.Equal(3u, results[only]);
    }

    [Fact]
    public void GetValues_AllowRepeats_RespectsQuantityMaximum()
    {
        WeightedItem capped = new("capped", 1f, 2u);
        List<WeightedItem> source = [capped];
        Dictionary<WeightedItem, uint> results = new();

        Weighted.GetValues(source, 10u, ref results, allowRepeatingEntries: true);

        Assert.Equal(2u, results[capped]);
    }

    [Fact]
    public void GetValues_NoRepeats_RespectsUniquePerEntry()
    {
        List<WeightedItem> source =
        [
            new("a", 1f, 10u),
            new("b", 1f, 10u),
            new("c", 1f, 10u),
        ];

        Dictionary<WeightedItem, uint> results = new();

        Weighted.GetValues(source, 3u, ref results, allowRepeatingEntries: false);

        Assert.Equal(3, results.Count);

        foreach (KeyValuePair<WeightedItem, uint> entry in results)
            Assert.Equal(1u, entry.Value);
    }

    [Fact]
    public void GetValues_NoRepeats_AddCountExceedsSource_StopsAtSourceCount()
    {
        List<WeightedItem> source =
        [
            new("a", 1f, 10u),
            new("b", 1f, 10u),
        ];

        Dictionary<WeightedItem, uint> results = new();

        Weighted.GetValues(source, 99u, ref results, allowRepeatingEntries: false);

        Assert.Equal(2, results.Count);

        foreach (KeyValuePair<WeightedItem, uint> entry in results)
            Assert.Equal(1u, entry.Value);
    }

    [Fact]
    public void GetValues_AllowRepeats_TotalPicksEqualAddCount_AcrossManySources()
    {
        List<WeightedItem> source =
        [
            new("a", 2f, 100u),
            new("b", 5f, 100u),
            new("c", 3f, 100u),
        ];

        Dictionary<WeightedItem, uint> results = new();

        Weighted.GetValues(source, 50u, ref results, allowRepeatingEntries: true);

        uint totalPicked = 0;
        foreach (KeyValuePair<WeightedItem, uint> entry in results)
            totalPicked += entry.Value;

        Assert.Equal(50u, totalPicked);
    }

    [Fact]
    public void GetValues_HighWeightItem_GetsPickedMoreOftenThanLowWeight()
    {
        WeightedItem heavy = new("heavy", 1000f, 10000u);
        WeightedItem light = new("light", 1f, 10000u);
        List<WeightedItem> source = [heavy, light];

        Dictionary<WeightedItem, uint> results = new();

        Weighted.GetValues(source, 500u, ref results, allowRepeatingEntries: true);

        results.TryGetValue(heavy, out uint heavyCount);
        results.TryGetValue(light, out uint lightCount);

        Assert.True(heavyCount > lightCount);
    }

    [Fact]
    public void GetValues_CountRangeOverload_RespectsRange()
    {
        WeightedItem only = new("only", 1f, 100u);
        List<WeightedItem> source = [only];
        Dictionary<WeightedItem, uint> results = new();

        Weighted.GetValues(source, new UIntRange(4u, 4u), ref results, allowRepeatingEntries: true);

        Assert.Equal(4u, results[only]);
    }
}
