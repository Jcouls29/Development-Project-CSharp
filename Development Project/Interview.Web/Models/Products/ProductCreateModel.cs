using System.Collections.Generic;
using System;

namespace Interview.Web.Models.Products
{
    public class ProductCreateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IReadOnlyList<string> ProductImageUris { get; set; }
        public IReadOnlyList<string> ValidSkus { get; set; }
        public IReadOnlyList<ProductMetadataModel> Metadata { get; set; }
        public IReadOnlyList<int> CategoryIds { get; set; }
    }

    public class ProductMetadataModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class ProductSearchRequestModel
    {
        public string SearchText { get; set; }
        public string MetadataKey { get; set; }
        public string MetadataValue { get; set; }
        public IReadOnlyList<int> CategoryIds { get; set; } = Array.Empty<int>();
    }

    public class ProductSearchResultModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IReadOnlyList<string> ProductImageUris { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> ValidSkus { get; set; } = Array.Empty<string>();
        public DateTime CreatedTimestamp { get; set; }
        public IReadOnlyList<ProductMetadataModel> Metadata { get; set; } = Array.Empty<ProductMetadataModel>();
        public IReadOnlyList<int> CategoryIds { get; set; } = Array.Empty<int>();
    }
}
