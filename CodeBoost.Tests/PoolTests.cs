using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CodeBoost.Performance;
using Xunit;

namespace CodeBoost.Tests;

public class PoolTests
{
    // ── ListPool ─────────────────────────────────────────────────────────

    [Fact]
    public void ListPool_RentReturnRent_ReusesClearedInstance()
    {
        List<int> first = ListPool<int>.Rent();
        first.Add(1);
        first.Add(2);

        ListPool<int>.Return(first);

        List<int> second = ListPool<int>.Rent();

        Assert.Empty(second);

        ListPool<int>.Return(second);
    }

    [Fact]
    public void ListPool_ReturnAndNullifyReference_NullsCallerVariable()
    {
        List<int>? rented = ListPool<int>.Rent();
        rented.Add(5);

        ListPool<int>.ReturnAndNullifyReference(ref rented!);

        Assert.Null(rented);
    }

    [Fact]
    public void ListPool_ReturnNull_NoOp()
    {
        List<int>? rented = null;

        ListPool<int>.Return(rented!);
    }

    // ── HashSetPool ──────────────────────────────────────────────────────

    [Fact]
    public void HashSetPool_RentReturnRent_ReusesClearedInstance()
    {
        HashSet<int> first = HashSetPool<int>.Rent();
        first.Add(1);
        first.Add(2);

        HashSetPool<int>.Return(first);

        HashSet<int> second = HashSetPool<int>.Rent();

        Assert.Empty(second);

        HashSetPool<int>.Return(second);
    }

    [Fact]
    public void HashSetPool_ReturnAndNullifyReference_NullsCallerVariable()
    {
        HashSet<int>? rented = HashSetPool<int>.Rent();
        rented.Add(5);

        HashSetPool<int>.ReturnAndNullifyReference(ref rented!);

        Assert.Null(rented);
    }

    // ── QueuePool ────────────────────────────────────────────────────────

    [Fact]
    public void QueuePool_RentReturnRent_ReusesClearedInstance()
    {
        Queue<int> first = QueuePool<int>.Rent();
        first.Enqueue(1);
        first.Enqueue(2);

        QueuePool<int>.Return(first);

        Queue<int> second = QueuePool<int>.Rent();

        Assert.Empty(second);

        QueuePool<int>.Return(second);
    }

    // ── DictionaryPool ───────────────────────────────────────────────────

    [Fact]
    public void DictionaryPool_RentReturnRent_ReusesClearedInstance()
    {
        Dictionary<int, string> first = DictionaryPool<int, string>.Rent();
        first[1] = "a";
        first[2] = "b";

        DictionaryPool<int, string>.Return(first);

        Dictionary<int, string> second = DictionaryPool<int, string>.Rent();

        Assert.Empty(second);

        DictionaryPool<int, string>.Return(second);
    }

    // ── SortedListPool ───────────────────────────────────────────────────

    [Fact]
    public void SortedListPool_RentReturnRent_ReusesClearedInstance()
    {
        SortedList<int, string> first = SortedListPool<int, string>.Rent();
        first[1] = "a";
        first[2] = "b";

        SortedListPool<int, string>.Return(first);

        SortedList<int, string> second = SortedListPool<int, string>.Rent();

        Assert.Empty(second);

        SortedListPool<int, string>.Return(second);
    }

    // ── ObjectPool ───────────────────────────────────────────────────────

    private sealed class PooledThing
    {
        public int Counter;
    }

    [Fact]
    public void ObjectPool_RentReturnRent_ReusesInstance()
    {
        PooledThing first = ObjectPool<PooledThing>.Rent();
        first.Counter = 99;

        ObjectPool<PooledThing>.Return(first);

        PooledThing second = ObjectPool<PooledThing>.Rent();

        // ObjectPool does not reset; instance should be reused with prior state.
        Assert.Same(first, second);

        ObjectPool<PooledThing>.Return(second);
    }

    // ── Utf8EncodingPool — concurrency safety ────────────────────────────

    [Fact]
    public void Utf8EncodingPool_RentAndReturn_DoesNotThrow()
    {
        UTF8Encoding encoding = Utf8EncodingPool.Rent();

        Assert.NotNull(encoding);

        Utf8EncodingPool.Return(encoding);
    }

    [Fact]
    public async Task Utf8EncodingPool_ConcurrentRentReturn_DoesNotCorrupt()
    {
        const int ThreadCount = 8;
        const int IterationsPerThread = 500;

        Task[] tasks = new Task[ThreadCount];

        for (int i = 0; i < ThreadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < IterationsPerThread; j++)
                {
                    UTF8Encoding encoding = Utf8EncodingPool.Rent();
                    Assert.NotNull(encoding);

                    byte[] bytes = encoding.GetBytes("test");
                    Assert.True(bytes.Length > 0);

                    Utf8EncodingPool.Return(encoding);
                }
            });
        }

        await Task.WhenAll(tasks);
    }

    [Fact]
    public void Utf8EncodingPool_ReturnAndNullifyReference_NullsCallerVariable()
    {
        UTF8Encoding? rented = Utf8EncodingPool.Rent();

        Utf8EncodingPool.ReturnAndNullifyReference(ref rented!);

        Assert.Null(rented);
    }

    [Fact]
    public void Utf8EncodingPool_ReturnNull_NoOp()
    {
        UTF8Encoding? value = null;

        Utf8EncodingPool.Return(value!);
    }

    // ── Wrapper caching parity (smoke test) ──────────────────────────────

    [Fact]
    public void Pools_RentReturnRent_DoNotThrowOnRepeatedCycles()
    {
        for (int i = 0; i < 100; i++)
        {
            List<int> list = ListPool<int>.Rent();
            HashSet<int> hashSet = HashSetPool<int>.Rent();
            Queue<int> queue = QueuePool<int>.Rent();
            Dictionary<int, int> dictionary = DictionaryPool<int, int>.Rent();

            list.Add(i);
            hashSet.Add(i);
            queue.Enqueue(i);
            dictionary[i] = i;

            ListPool<int>.Return(list);
            HashSetPool<int>.Return(hashSet);
            QueuePool<int>.Return(queue);
            DictionaryPool<int, int>.Return(dictionary);
        }
    }
}
