using System.Collections.Generic;
using System.Linq;
using CodeBoost.Types;
using Xunit;

namespace CodeBoost.Tests;

public class RoundRobinCursorTests
{
    [Fact]
    public void Uninitialized_Enumerate_YieldsEmpty()
    {
        RoundRobinCursor<int> cursor = new();

        IEnumerable<int> result = cursor.Enumerate(100u);

        Assert.Empty(result);
    }

    [Fact]
    public void Initialize_NullCollection_LeavesCursorUninitialized()
    {
        RoundRobinCursor<int> cursor = new();

        cursor.Initialize(null!, 1000u);

        Assert.Empty(cursor.Enumerate(100u));
    }

    [Fact]
    public void Initialize_ZeroSweepWindow_LeavesCursorUninitialized()
    {
        RoundRobinCursor<int> cursor = new();

        cursor.Initialize(new List<int> { 1, 2, 3 }, 0u);

        Assert.Empty(cursor.Enumerate(100u));
    }

    [Fact]
    public void Enumerate_EmptyCollection_YieldsEmpty()
    {
        RoundRobinCursor<int> cursor = new(new List<int>(), 1000u);

        Assert.Empty(cursor.Enumerate(500u));
    }

    [Fact]
    public void Enumerate_BatchSizeFloorsToOne_OnSmallElapsed()
    {
        List<int> source = [1, 2, 3, 4];
        RoundRobinCursor<int> cursor = new(source, 1000u);

        int[] firstBatch = cursor.Enumerate(1u).ToArray();
        int[] secondBatch = cursor.Enumerate(1u).ToArray();

        Assert.Single(firstBatch);
        Assert.Single(secondBatch);
        Assert.Equal(1, firstBatch[0]);
        Assert.Equal(2, secondBatch[0]);
    }

    [Fact]
    public void Enumerate_BatchScaledByElapsed_OverFullSweep()
    {
        List<int> source = [10, 20, 30, 40];
        RoundRobinCursor<int> cursor = new(source, 1000u);

        int[] batch = cursor.Enumerate(1000u).ToArray();

        Assert.Equal(source, batch);
    }

    [Fact]
    public void Enumerate_BatchClampedToCollectionCount_OnExcessElapsed()
    {
        List<int> source = [1, 2, 3];
        RoundRobinCursor<int> cursor = new(source, 1000u);

        int[] batch = cursor.Enumerate(99999u).ToArray();

        Assert.Equal(source.Count, batch.Length);
    }

    [Fact]
    public void Enumerate_WrapsAroundAfterEndOfCollection()
    {
        List<int> source = [1, 2, 3];
        RoundRobinCursor<int> cursor = new(source, 1000u);

        cursor.Enumerate(1000u).ToArray();

        int[] secondPass = cursor.Enumerate(1000u).ToArray();

        Assert.Equal(source, secondPass);
    }

    [Fact]
    public void EnsureSweepWindow_ZeroValue_FailsAndLogs()
    {
        RoundRobinCursor<int> cursor = new(new List<int> { 1 }, 1000u);

        bool isUpdated = cursor.EnsureSweepWindow(0u);

        Assert.False(isUpdated);
    }

    [Fact]
    public void EnsureSweepWindow_NonZero_Succeeds()
    {
        RoundRobinCursor<int> cursor = new(new List<int> { 1 }, 1000u);

        bool isUpdated = cursor.EnsureSweepWindow(500u);

        Assert.True(isUpdated);
    }

    [Fact]
    public void OnReturn_RestoresUninitializedState()
    {
        RoundRobinCursor<int> cursor = new(new List<int> { 1, 2, 3 }, 1000u);

        cursor.OnReturn();

        Assert.Empty(cursor.Enumerate(100u));
    }

    [Fact]
    public void Enumerate_ReturnsBatchEnumerableStruct()
    {
        RoundRobinCursor<int> cursor = new(new List<int> { 1, 2, 3 }, 1000u);

        RoundRobinCursor<int>.BatchEnumerable batch = cursor.Enumerate(500u);
        RoundRobinCursor<int>.BatchEnumerator enumerator = batch.GetEnumerator();

        int collected = 0;
        while (enumerator.MoveNext())
            collected++;

        Assert.True(collected > 0);
    }

    [Fact]
    public void DefaultBatchEnumerable_YieldsNothing()
    {
        RoundRobinCursor<int>.BatchEnumerable batch = default;

        int collected = 0;
        foreach (int unused in batch)
            collected++;

        Assert.Equal(0, collected);
    }

    [Fact]
    public void Foreach_OverEnumerate_AdvancesLastIndex()
    {
        List<int> source = [10, 20, 30, 40, 50];
        RoundRobinCursor<int> cursor = new(source, 1000u);

        List<int> firstPass = new();
        foreach (int value in cursor.Enumerate(400u))
            firstPass.Add(value);

        List<int> secondPass = new();
        foreach (int value in cursor.Enumerate(400u))
            secondPass.Add(value);

        Assert.Equal(2, firstPass.Count);
        Assert.Equal(2, secondPass.Count);
        Assert.Equal(10, firstPass[0]);
        Assert.Equal(20, firstPass[1]);
        Assert.Equal(30, secondPass[0]);
        Assert.Equal(40, secondPass[1]);
    }

    [Fact]
    public void Foreach_PartialConsumption_ReflectsAdvanceOnCursor()
    {
        List<int> source = [1, 2, 3, 4, 5];
        RoundRobinCursor<int> cursor = new(source, 1000u);

        RoundRobinCursor<int>.BatchEnumerator enumerator = cursor.Enumerate(1000u).GetEnumerator();
        enumerator.MoveNext();
        enumerator.MoveNext();

        List<int> remainder = new();
        foreach (int value in cursor.Enumerate(600u))
            remainder.Add(value);

        Assert.Equal(3, remainder.Count);
        Assert.Equal(3, remainder[0]);
        Assert.Equal(4, remainder[1]);
        Assert.Equal(5, remainder[2]);
    }
}
