using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sparcpoint.Core.Models;
using Sparcpoint.Core.Repositories;
using Sparcpoint.Core.Services.Implementations;
using Xunit;

namespace Sparcpoint.Core.Tests
{
    public class InventoryServiceTests
    {
        private class FakeProductRepository : IProductRepository
        {
            private readonly Dictionary<int, Product> _products = new Dictionary<int, Product>();
            private readonly Dictionary<int, decimal> _inventoryCounts = new Dictionary<int, decimal>();

            public void Seed(Product product, decimal inventoryCount = 0m)
            {
                _products[product.InstanceId] = product;
                _inventoryCounts[product.InstanceId] = inventoryCount;
            }

            public Task<int> AddAsync(Product product) => Task.FromResult(1);

            public Task<decimal> GetCurrentInventoryCountAsync(int productInstanceId)
            {
                _inventoryCounts.TryGetValue(productInstanceId, out var count);
                return Task.FromResult(count);
            }

            public Task<IEnumerable<Product>> GetAllAsync() => Task.FromResult<IEnumerable<Product>>(_products.Values);

            public Task<Product> GetByIdAsync(int instanceId) => Task.FromResult(_products.TryGetValue(instanceId, out var product) ? product : null);

            public Task<int> GetSearchCountAsync(string name = null, string description = null, IEnumerable<int> categoryIds = null, Dictionary<string, string> metadataFilters = null)
            {
                return Task.FromResult(_products.Count);
            }

            public Task<IEnumerable<Product>> SearchAsync(string name = null, string description = null, IEnumerable<int> categoryIds = null, Dictionary<string, string> metadataFilters = null, int? skip = null, int? take = null)
            {
                return Task.FromResult<IEnumerable<Product>>(_products.Values);
            }

            public Task UpdateAsync(Product product)
            {
                _products[product.InstanceId] = product;
                return Task.CompletedTask;
            }
        }

        private class FakeInventoryTransactionRepository : IInventoryTransactionRepository
        {
            private readonly Dictionary<int, InventoryTransaction> _transactions = new Dictionary<int, InventoryTransaction>();
            private int _nextId = 1;
            public readonly List<InventoryTransaction> AddedTransactions = new List<InventoryTransaction>();

            public Task<int> AddAsync(InventoryTransaction transaction)
            {
                var id = _nextId++;
                _transactions[id] = transaction;
                AddedTransactions.Add(transaction);
                return Task.FromResult(id);
            }

            public void Seed(InventoryTransaction transaction)
            {
                _transactions[transaction.TransactionId] = transaction;
            }

            public void Seed(InventoryTransaction transaction, int transactionId)
            {
                _transactions[transactionId] = transaction;
            }

            public Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(int productInstanceId)
            {
                var results = _transactions.Values.Where(t => t.ProductInstanceId == productInstanceId);
                return Task.FromResult<IEnumerable<InventoryTransaction>>(results.ToList());
            }

            public Task<InventoryTransaction> GetByIdAsync(int transactionId)
            {
                _transactions.TryGetValue(transactionId, out var transaction);
                return Task.FromResult(transaction);
            }

            public Task<IEnumerable<InventoryTransaction>> GetUncompletedTransactionsAsync()
            {
                var results = _transactions.Values.Where(t => !t.CompletedTimestamp.HasValue).ToList();
                return Task.FromResult<IEnumerable<InventoryTransaction>>(results);
            }

            public Task CompleteTransactionAsync(int transactionId)
            {
                if (_transactions.TryGetValue(transactionId, out var transaction) && !transaction.CompletedTimestamp.HasValue)
                {
                    transaction.Complete();
                }
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task AddInventoryAsync_ReturnsTransactionId_ForValidProduct()
        {
            var productRepository = new FakeProductRepository();
            var product = Product.Create("Test Product", "Desc", null, null);
            product.SetInstanceId(1);
            productRepository.Seed(product);

            var inventoryService = new InventoryService(new FakeInventoryTransactionRepository(), productRepository);

            var transactionId = await inventoryService.AddInventoryAsync(1, 5, "MANUAL");

            Assert.Equal(1, transactionId);
        }

        [Fact]
        public async Task RemoveInventoryAsync_Throws_WhenInsufficientInventory()
        {
            var productRepository = new FakeProductRepository();
            var product = Product.Create("Test Product", "Desc", null, null);
            product.SetInstanceId(2);
            productRepository.Seed(product);

            var inventoryService = new InventoryService(new FakeInventoryTransactionRepository(), productRepository);

            await Assert.ThrowsAsync<InvalidOperationException>(() => inventoryService.RemoveInventoryAsync(2, 200m, "REMOVE"));
        }

        [Fact]
        public async Task AddInventoryBulk_ReturnsAllTransactionIds()
        {
            var productRepository = new FakeProductRepository();
            var product1 = Product.Create("Test Product 1", "Desc 1", null, null);
            product1.SetInstanceId(10);
            var product2 = Product.Create("Test Product 2", "Desc 2", null, null);
            product2.SetInstanceId(20);
            productRepository.Seed(product1);
            productRepository.Seed(product2);

            var inventoryService = new InventoryService(new FakeInventoryTransactionRepository(), productRepository);

            var adjustments = new List<InventoryAdjustment>
            {
                new InventoryAdjustment { ProductInstanceId = 10, Quantity = 5, TypeCategory = "ADD" },
                new InventoryAdjustment { ProductInstanceId = 20, Quantity = 2, TypeCategory = "ADD" }
            };

            var transactionIds = await inventoryService.AddInventoryAsync(adjustments);

            Assert.Collection(transactionIds,
                id => Assert.Equal(1, id),
                id => Assert.Equal(2, id));
        }

        [Fact]
        public async Task UndoTransactionAsync_AddsCompensatingTransaction()
        {
            var fakeRepo = new FakeInventoryTransactionRepository();
            var productRepository = new FakeProductRepository();
            var product = Product.Create("Test Product", "Desc", null, null);
            product.SetInstanceId(5);
            productRepository.Seed(product);

            var originalTransaction = InventoryTransaction.Create(5, 10m, "ADD");
            originalTransaction.Complete();
            fakeRepo.Seed(originalTransaction, 100);

            var inventoryService = new InventoryService(fakeRepo, productRepository);
            var result = await inventoryService.UndoTransactionAsync(100);

            Assert.True(result);
            Assert.Contains(fakeRepo.AddedTransactions, t => t.Quantity == -10m && t.ProductInstanceId == 5 && t.TypeCategory == "UNDO");
        }

        [Fact]
        public async Task GetInventoryCountByFilterAsync_ReturnsSumOfInventoryForSearchResults()
        {
            var productRepository = new FakeProductRepository();
            var product1 = Product.Create("Product A", "Desc A", null, null);
            product1.SetInstanceId(11);
            var product2 = Product.Create("Product B", "Desc B", null, null);
            product2.SetInstanceId(12);
            productRepository.Seed(product1, 7m);
            productRepository.Seed(product2, 3m);

            var inventoryService = new InventoryService(new FakeInventoryTransactionRepository(), productRepository);
            var total = await inventoryService.GetInventoryCountByFilterAsync("Product", null, null, null);

            Assert.Equal(10m, total);
        }
    }
}
