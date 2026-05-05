using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Tests.Core
{
    public class BlockingCollectionAsyncQueueTests
    {
        private static BlockingCollectionAsyncQueue<string> Queue(int timeoutMs = 500)
            => new BlockingCollectionAsyncQueue<string>(TimeSpan.FromMilliseconds(timeoutMs));

        // ── Constructor ───────────────────────────────────────────────────────

        [Fact]
        public void Constructor_WithZeroTimeout_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new BlockingCollectionAsyncQueue<string>(TimeSpan.Zero));
        }

        [Fact]
        public void Constructor_WithNegativeTimeout_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new BlockingCollectionAsyncQueue<string>(TimeSpan.FromSeconds(-1)));
        }

        [Fact]
        public void Constructor_WithValidTimeout_DoesNotThrow()
        {
            using var q = Queue(100);
            // no exception
        }

        // ── Add ───────────────────────────────────────────────────────────────

        [Fact]
        public void Add_NullEntry_ThrowsArgumentNullException()
        {
            using var q = Queue();

            Assert.Throws<ArgumentNullException>(() => q.Add(null!));
        }

        [Fact]
        public void Add_ValidEntry_DoesNotThrow()
        {
            using var q = Queue();
            q.Add("hello");
        }

        // ── Take ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Take_AfterAdd_ReturnsAddedItems()
        {
            using var q = Queue(1000);
            q.Add("first");
            q.Add("second");

            var results = (await q.Take(2, CancellationToken.None)).ToList();

            Assert.Equal(2, results.Count);
            Assert.Contains("first",  results);
            Assert.Contains("second", results);
        }

        [Fact]
        public async Task Take_WhenQueueEmpty_ReturnsEmptyAfterTimeout()
        {
            using var q = Queue(150);

            var results = await q.Take(5, CancellationToken.None);

            Assert.Empty(results);
        }

        [Fact]
        public async Task Take_WithCancellationAlreadyRequested_ReturnsEmpty()
        {
            using var q = Queue(2000);
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            // Should return immediately without waiting for timeout
            var results = await q.Take(5, cts.Token);

            Assert.Empty(results);
        }

        [Fact]
        public async Task Take_PartialFill_ReturnsAvailableItems()
        {
            using var q = Queue(300);
            q.Add("only-one");

            // Ask for 5, but only 1 is available — should return 1 after timeout
            var results = (await q.Take(5, CancellationToken.None)).ToList();

            Assert.Single(results);
            Assert.Equal("only-one", results[0]);
        }
    }
}
