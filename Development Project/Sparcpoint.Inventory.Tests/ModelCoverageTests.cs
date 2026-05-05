// EVAL: Additional model tests to ensure 100% coverage on DTOs and response models.

using Interview.Web.Models.Requests;
using Interview.Web.Models.Responses;
using Sparcpoint.Inventory.Models;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Tests
{
    public class ModelCoverageTests
    {
        #region Request Models

        [Fact]
        public void RemoveInventoryRequest_DefaultItems_IsEmptyList()
        {
            var request = new RemoveInventoryRequest();

            Assert.NotNull(request.Items);
            Assert.Empty(request.Items);
        }

        [Fact]
        public void SearchProductsRequest_DefaultValues()
        {
            var request = new SearchProductsRequest();

            Assert.Null(request.Name);
            Assert.Null(request.Description);
            Assert.Null(request.CategoryIds);
            Assert.Null(request.Attributes);
            Assert.Equal(0, request.Skip);
            Assert.Equal(50, request.Take);
        }

        [Fact]
        public void InventoryTransactionItem_Properties_SetAndGet()
        {
            var item = new Interview.Web.Models.Requests.InventoryTransactionItem
            {
                ProductInstanceId = 42,
                Quantity = 99.5m,
                TypeCategory = "Return"
            };

            Assert.Equal(42, item.ProductInstanceId);
            Assert.Equal(99.5m, item.Quantity);
            Assert.Equal("Return", item.TypeCategory);
        }

        #endregion

        #region Response Models

        [Fact]
        public void CategorySummary_Properties_SetAndGet()
        {
            var summary = new CategorySummary
            {
                InstanceId = 5,
                Name = "Electronics"
            };

            Assert.Equal(5, summary.InstanceId);
            Assert.Equal("Electronics", summary.Name);
        }

        [Fact]
        public void ProductResponse_DefaultCollections_NotNull()
        {
            var response = new ProductResponse();

            Assert.NotNull(response.ImageUris);
            Assert.NotNull(response.Skus);
            Assert.NotNull(response.Attributes);
            Assert.NotNull(response.Categories);
        }

        [Fact]
        public void ErrorResponse_AllProperties_SetAndGet()
        {
            var error = new ErrorResponse
            {
                Code = "TEST_ERROR",
                Message = "Something happened",
                Details = new Dictionary<string, string[]>
                {
                    { "Name", new[] { "Name is required" } }
                }
            };

            Assert.Equal("TEST_ERROR", error.Code);
            Assert.Equal("Something happened", error.Message);
            Assert.Single(error.Details);
        }

        [Fact]
        public void ProductInventoryCount_Properties_SetAndGet()
        {
            var count = new ProductInventoryCount
            {
                ProductInstanceId = 1,
                ProductName = "Galaxy S24",
                Count = 41m
            };

            Assert.Equal(1, count.ProductInstanceId);
            Assert.Equal("Galaxy S24", count.ProductName);
            Assert.Equal(41m, count.Count);
        }

        [Fact]
        public void InventoryCountResponse_DefaultItems_NotNull()
        {
            var response = new InventoryCountResponse();

            Assert.NotNull(response.Items);
            Assert.Equal(0m, response.TotalCount);
        }

        [Fact]
        public void TransactionDetail_Properties_SetAndGet()
        {
            var detail = new TransactionDetail
            {
                TransactionId = 1,
                ProductInstanceId = 2,
                Quantity = 25m,
                TypeCategory = "Purchase",
                StartedTimestamp = System.DateTime.UtcNow,
                IsActive = true
            };

            Assert.Equal(1, detail.TransactionId);
            Assert.Equal(2, detail.ProductInstanceId);
            Assert.Equal(25m, detail.Quantity);
            Assert.True(detail.IsActive);
        }

        [Fact]
        public void InventoryTransactionResponse_DefaultTransactions_NotNull()
        {
            var response = new InventoryTransactionResponse();

            Assert.NotNull(response.Transactions);
            Assert.Empty(response.Transactions);
        }

        #endregion

        #region Domain Models

        [Fact]
        public void Category_AllProperties_SetAndGet()
        {
            var category = new Category
            {
                InstanceId = 1,
                Name = "Electronics",
                Description = "Electronic devices",
                CreatedTimestamp = System.DateTime.UtcNow,
                Attributes = new Dictionary<string, string> { { "Department", "Tech" } },
                ParentCategoryIds = new List<int> { 10 }
            };

            Assert.Equal(1, category.InstanceId);
            Assert.Equal("Electronics", category.Name);
            Assert.Equal("Electronic devices", category.Description);
            Assert.Single(category.Attributes);
            Assert.Single(category.ParentCategoryIds);
        }

        [Fact]
        public void Product_AllProperties_SetAndGet()
        {
            var product = new Product
            {
                InstanceId = 1,
                Name = "Test",
                Description = "Desc",
                ProductImageUris = new List<string> { "https://example.com/img.jpg" },
                ValidSkus = new List<string> { "SKU-1" },
                CreatedTimestamp = System.DateTime.UtcNow,
                Attributes = new Dictionary<string, string> { { "Color", "Red" } },
                CategoryIds = new List<int> { 1 },
                Categories = new Dictionary<int, string> { { 1, "Electronics" } }
            };

            Assert.Equal(1, product.InstanceId);
            Assert.Single(product.ProductImageUris);
            Assert.Single(product.ValidSkus);
            Assert.Single(product.Categories);
        }

        [Fact]
        public void Product_DefaultCategories_IsEmptyDictionary()
        {
            var product = new Product();

            Assert.NotNull(product.Categories);
            Assert.Empty(product.Categories);
        }

        #endregion
    }
}
