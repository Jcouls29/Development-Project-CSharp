using System.Linq;
using Interview.Web.Models.Requests;
using Sparcpoint.Inventory.Models;

namespace Interview.Web.Models
{
    /// <summary>
    /// EVAL: Extension methods to map DTOs from API -> domain models.
    /// This way controllers stay thin and the domain doesn't depend on the HTTP contract.
    /// </summary>
    internal static class RequestMappingExtensions
    {
        public static Product ToDomain(this AddProductRequest req) => new Product
        {
            Name = req.Name,
            Description = req.Description,
            ProductImageUris = req.ProductImageUris?.ToList() ?? new System.Collections.Generic.List<string>(),
            ValidSkus = req.ValidSkus?.ToList() ?? new System.Collections.Generic.List<string>(),
            Attributes = req.Attributes?
                .Select(a => new ProductAttribute(a.Key, a.Value)).ToList<ProductAttribute>()
                ?? new System.Collections.Generic.List<ProductAttribute>(),
            CategoryIds = req.CategoryIds?.ToList() ?? new System.Collections.Generic.List<int>()
        };

        public static Category ToDomain(this AddCategoryRequest req) => new Category
        {
            Name = req.Name,
            Description = req.Description,
            Attributes = req.Attributes?
                .Select(a => new CategoryAttribute(a.Key, a.Value)).ToList<CategoryAttribute>()
                ?? new System.Collections.Generic.List<CategoryAttribute>(),
            ParentCategoryIds = req.ParentCategoryIds?.ToList() ?? new System.Collections.Generic.List<int>()
        };

        public static ProductSearchCriteria ToDomain(this SearchProductsRequest req) => new ProductSearchCriteria
        {
            NameContains = req.NameContains,
            AttributeFilters = req.AttributeFilters?
                .Select(a => new ProductAttribute(a.Key, a.Value)).ToList<ProductAttribute>()
                ?? new System.Collections.Generic.List<ProductAttribute>(),
            CategoryIds = req.CategoryIds?.ToList() ?? new System.Collections.Generic.List<int>(),
            Skip = req.Skip,
            Take = req.Take
        };

        public static InventoryAdjustment ToDomain(this AdjustInventoryItem item) =>
            new InventoryAdjustment(item.ProductInstanceId, item.Quantity, item.TypeCategory);

        public static InventoryCountQuery ToDomain(this GetInventoryCountsRequest req) => new InventoryCountQuery
        {
            ProductInstanceId = req.ProductInstanceId,
            AttributeFilters = req.AttributeFilters?
                .Select(a => new ProductAttribute(a.Key, a.Value)).ToList<ProductAttribute>()
                ?? new System.Collections.Generic.List<ProductAttribute>(),
            CategoryIds = req.CategoryIds?.ToList() ?? new System.Collections.Generic.List<int>(),
            IncludeReverted = req.IncludeReverted
        };
    }
}
