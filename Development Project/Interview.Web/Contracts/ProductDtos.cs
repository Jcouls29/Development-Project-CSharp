using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Interview.Web.Contracts
{
    public sealed class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string[]? ProductImageUris { get; set; }
        public string[]? ValidSkus { get; set; }
        public Dictionary<string, string>? Attributes { get; set; }
        public int[]? CategoryIds { get; set; }

        public Product ToModel() => new()
        {
            Name = Name,
            Description = Description,
            ProductImageUris = ProductImageUris ?? Array.Empty<string>(),
            ValidSkus = ValidSkus ?? Array.Empty<string>(),
            Attributes = Attributes?.Select(kv => new ProductAttribute(kv.Key, kv.Value)).ToArray() ?? Array.Empty<ProductAttribute>(),
            CategoryIds = CategoryIds ?? Array.Empty<int>(),
        };
    }

    public sealed class ProductSearchRequest
    {
        public string? NameContains { get; set; }
        public string? DescriptionContains { get; set; }
        public int[]? CategoryIds { get; set; }
        public Dictionary<string, string>? Attributes { get; set; }
        public string? Sku { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 50;

        public ProductSearchCriteria ToCriteria() => new()
        {
            NameContains = NameContains,
            DescriptionContains = DescriptionContains,
            CategoryIds = CategoryIds ?? Array.Empty<int>(),
            AttributeMatches = Attributes?.Select(kv => new ProductAttribute(kv.Key, kv.Value)).ToArray() ?? Array.Empty<ProductAttribute>(),
            Sku = Sku,
            Skip = Skip,
            Take = Take,
        };
    }
}
