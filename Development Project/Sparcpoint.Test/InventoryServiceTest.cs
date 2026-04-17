using Sparcpoint.Domain;
using Sparcpoint.Implementations.Services;

namespace Sparcpoint.Test
{
    public class InventoryServiceTest
    {
        private readonly InventoryService _service;

        public InventoryServiceTest()
        {

            _service = new InventoryService(null);
        }

        [Fact]
        public void CalculateStock_ShouldSumCorrectly()
        {
            // ARRANGE
            var transactions = new List<InventoryTransaction>
            {
                new InventoryTransaction { Quantity = 10m },
                new InventoryTransaction { Quantity = -3.5m }
            };

            // ACT
            var result = _service.CalculateCurrentStock(transactions);

            // ASSERT
            Assert.Equal(6.5m, result);
        }

        [Fact]
        public void CalculateStock_WithEmptyList_ReturnsZero()
        {
            // ARRANGE
            var transactions = new List<InventoryTransaction>();

            // ACT
            var result = _service.CalculateCurrentStock(transactions);

            // ASSERT
            Assert.Equal(0, result);
        }
    }
}