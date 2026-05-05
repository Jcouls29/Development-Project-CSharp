// EVAL: Tests for domain model logic (no mocks needed).
// Validates computed properties and business rules on the models themselves.

using Sparcpoint.Inventory.Models;
using System;

namespace Sparcpoint.Inventory.Tests
{
    public class InventoryTransactionModelTests
    {
        [Fact]
        public void IsActive_CompletedTimestampNull_ReturnsTrue()
        {
            // Arrange
            var transaction = new InventoryTransaction
            {
                TransactionId = 1,
                CompletedTimestamp = null
            };

            // Assert
            Assert.True(transaction.IsActive);
        }

        [Fact]
        public void IsActive_CompletedTimestampSet_ReturnsFalse()
        {
            // Arrange
            // RV: Transaction with CompletedTimestamp = undone, should not be active
            var transaction = new InventoryTransaction
            {
                TransactionId = 1,
                CompletedTimestamp = DateTime.UtcNow
            };

            // Assert
            Assert.False(transaction.IsActive);
        }

        [Fact]
        public void Product_DefaultAttributes_IsEmptyDictionary()
        {
            // Arrange / Act
            var product = new Product();

            // Assert
            // EVAL: Default collections should be empty, not null,
            // to prevent NullReferenceExceptions downstream.
            Assert.NotNull(product.Attributes);
            Assert.Empty(product.Attributes);
        }

        [Fact]
        public void Product_DefaultCategoryIds_IsEmptyList()
        {
            // Arrange / Act
            var product = new Product();

            // Assert
            Assert.NotNull(product.CategoryIds);
            Assert.Empty(product.CategoryIds);
        }

        [Fact]
        public void Category_DefaultAttributes_IsEmptyDictionary()
        {
            // Arrange / Act
            var category = new Category();

            // Assert
            Assert.NotNull(category.Attributes);
            Assert.Empty(category.Attributes);
        }

        [Fact]
        public void Category_DefaultParentCategoryIds_IsEmptyList()
        {
            // Arrange / Act
            var category = new Category();

            // Assert
            Assert.NotNull(category.ParentCategoryIds);
            Assert.Empty(category.ParentCategoryIds);
        }
    }
}
