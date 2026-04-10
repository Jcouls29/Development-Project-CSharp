using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Repositories.InMemory
{
    /// <summary>
    /// EVAL: Thread-safe in-memory implementation of IInventoryRepository.
    /// Mirrors the transaction-based approach of the SQL implementation.
    /// Supports attribute-based inventory counts by accepting a product attribute lookup function.
    /// </summary>
    public class InMemoryInventoryRepository : IInventoryRepository
    {
        private readonly List<InventoryTransaction> _Transactions = new();
        private readonly object _Lock = new();
        private int _NextId = 1;

        private readonly IProductRepository _ProductRepository;

        /// <summary>
        /// EVAL: Accepts IProductRepository to enable attribute-based inventory count queries.
        /// In the SQL implementation this is done via a JOIN; here we look up attributes in-memory.
        /// </summary>
        public InMemoryInventoryRepository(IProductRepository productRepository)
        {
            _ProductRepository = productRepository;
        }

        public Task<InventoryTransaction> AddTransactionAsync(InventoryTransaction transaction)
        {
            lock (_Lock)
            {
                transaction.TransactionId = _NextId++;
                _Transactions.Add(transaction);
            }

            return Task.FromResult(transaction);
        }

        /// <summary>
        /// EVAL: Soft delete — sets CompletedTimestamp to null instead of removing the record.
        /// Mirrors the SQL implementation. Already-undone transactions return false.
        /// </summary>
        public Task<bool> RemoveTransactionAsync(int transactionId)
        {
            lock (_Lock)
            {
                var transaction = _Transactions.FirstOrDefault(t => t.TransactionId == transactionId && t.CompletedTimestamp.HasValue);
                if (transaction == null)
                    return Task.FromResult(false);

                transaction.CompletedTimestamp = null;
                return Task.FromResult(true);
            }
        }

        public Task<decimal> GetInventoryCountAsync(int productInstanceId)
        {
            lock (_Lock)
            {
                var count = _Transactions
                    .Where(t => t.ProductInstanceId == productInstanceId && t.CompletedTimestamp.HasValue)
                    .Sum(t => t.Quantity);

                return Task.FromResult(count);
            }
        }

        public async Task<IEnumerable<InventoryCountSummary>> GetInventoryCountsByAttributeAsync(string key, string value)
        {
            List<InventoryTransaction> snapshot;
            lock (_Lock)
            {
                snapshot = _Transactions.Where(t => t.CompletedTimestamp.HasValue).ToList();
            }

            // EVAL: Find which products have the matching attribute
            var productIds = snapshot.Select(t => t.ProductInstanceId).Distinct();
            var matchingProductIds = new List<int>();

            foreach (var productId in productIds)
            {
                var product = await _ProductRepository.GetByIdAsync(productId);
                if (product?.Attributes != null &&
                    product.Attributes.TryGetValue(key, out var attrValue) &&
                    attrValue == value)
                {
                    matchingProductIds.Add(productId);
                }
            }

            var results = snapshot
                .Where(t => matchingProductIds.Contains(t.ProductInstanceId))
                .GroupBy(t => t.ProductInstanceId)
                .Select(g => new InventoryCountSummary
                {
                    ProductInstanceId = g.Key,
                    Count = g.Sum(t => t.Quantity)
                });

            return results;
        }
    }
}
