using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Services;
using Sparcpoint.Inventory.Tests.Fakes;
using Xunit;

namespace Sparcpoint.Inventory.Tests.Services
{
    /// <summary>
    /// EVAL: Unit tests for InventoryService demonstrating transaction-based
    /// inventory management and undo capability.
    /// </summary>
    public class InventoryServiceTests
    {
        private readonly InMemoryProductRepository _ProductRepository;
        private readonly InMemoryInventoryRepository _InventoryRepository;
        private readonly InventoryService _Service;

        public InventoryServiceTests()
        {
            _ProductRepository = new InMemoryProductRepository();
            _InventoryRepository = new InMemoryInventoryRepository(_ProductRepository);
            _Service = new InventoryService(_InventoryRepository, _ProductRepository);
        }

        private async Task<Product> CreateTestProduct()
        {
            return await _ProductRepository.CreateAsync(new Product
            {
                Name = "Test Product",
                Description = "For testing",
                ProductImageUris = "",
                ValidSkus = ""
            });
        }

        [Fact]
        public async Task AddToInventory_WithValidData_CreatesPositiveTransaction()
        {
            var product = await CreateTestProduct();

            var transaction = await _Service.AddToInventoryAsync(product.InstanceId, 10);

            Assert.True(transaction.TransactionId > 0);
            Assert.Equal(product.InstanceId, transaction.ProductInstanceId);
            Assert.Equal(10, transaction.Quantity);
            Assert.NotNull(transaction.CompletedTimestamp);
        }

        [Fact]
        public async Task RemoveFromInventory_WithValidData_CreatesNegativeTransaction()
        {
            var product = await CreateTestProduct();

            var transaction = await _Service.RemoveFromInventoryAsync(product.InstanceId, 5);

            Assert.Equal(-5, transaction.Quantity);
        }

        [Fact]
        public async Task AddToInventory_WithZeroQuantity_ThrowsArgumentException()
        {
            var product = await CreateTestProduct();

            await Assert.ThrowsAsync<ArgumentException>(
                () => _Service.AddToInventoryAsync(product.InstanceId, 0));
        }

        [Fact]
        public async Task AddToInventory_WithNegativeQuantity_ThrowsArgumentException()
        {
            var product = await CreateTestProduct();

            await Assert.ThrowsAsync<ArgumentException>(
                () => _Service.AddToInventoryAsync(product.InstanceId, -5));
        }

        [Fact]
        public async Task AddToInventory_WithNonExistentProduct_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _Service.AddToInventoryAsync(999, 10));
        }

        [Fact]
        public async Task RemoveFromInventory_WithNonExistentProduct_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _Service.RemoveFromInventoryAsync(999, 5));
        }

        [Fact]
        public async Task GetInventoryCount_AfterAddAndRemove_ReturnsNetCount()
        {
            var product = await CreateTestProduct();

            await _Service.AddToInventoryAsync(product.InstanceId, 100);
            await _Service.AddToInventoryAsync(product.InstanceId, 50);
            await _Service.RemoveFromInventoryAsync(product.InstanceId, 30);

            var count = await _Service.GetInventoryCountAsync(product.InstanceId);

            Assert.Equal(120, count); // 100 + 50 - 30
        }

        [Fact]
        public async Task UndoTransaction_RemovesTransactionAndAffectsCount()
        {
            var product = await CreateTestProduct();

            var add1 = await _Service.AddToInventoryAsync(product.InstanceId, 100);
            var add2 = await _Service.AddToInventoryAsync(product.InstanceId, 50);

            // Undo the second addition
            var undone = await _Service.UndoTransactionAsync(add2.TransactionId);
            Assert.True(undone);

            var count = await _Service.GetInventoryCountAsync(product.InstanceId);
            Assert.Equal(100, count); // Only first addition remains
        }

        [Fact]
        public async Task UndoTransaction_WithNonExistentId_ReturnsFalse()
        {
            var result = await _Service.UndoTransactionAsync(999);
            Assert.False(result);
        }

        [Fact]
        public async Task UndoTransaction_WithInvalidId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _Service.UndoTransactionAsync(0));
        }

        [Fact]
        public async Task GetInventoryCount_WithNoTransactions_ReturnsZero()
        {
            var product = await CreateTestProduct();

            var count = await _Service.GetInventoryCountAsync(product.InstanceId);

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task AddToInventory_WithTypeCategory_SetsTypeCategory()
        {
            var product = await CreateTestProduct();

            var transaction = await _Service.AddToInventoryAsync(product.InstanceId, 10, "Purchase");

            Assert.Equal("Purchase", transaction.TypeCategory);
        }

        [Fact]
        public async Task GetInventoryCountsByAttribute_WithEmptyKey_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _Service.GetInventoryCountsByAttributeAsync("", "Red"));
        }

        [Fact]
        public async Task GetInventoryCountsByAttribute_WithEmptyValue_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _Service.GetInventoryCountsByAttributeAsync("Color", ""));
        }
    }
}
