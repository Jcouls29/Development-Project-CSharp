using System;
using System.Collections.Generic;

namespace Interview.Web.Contracts.Products
{
    public class SearchProductsResponse
    {
        public List<SearchProductItemResponse> Products { get; set; } = new List<SearchProductItemResponse>();
    }

    public class SearchProductItemResponse
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IReadOnlyList<string> ProductImageUris { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> ValidSkus { get; set; } = Array.Empty<string>();
        public DateTime CreatedTimestamp { get; set; }
        public IReadOnlyList<SearchProductMetadataResponse> Metadata { get; set; } = Array.Empty<SearchProductMetadataResponse>();
        public IReadOnlyList<int> CategoryIds { get; set; } = Array.Empty<int>();
    }

    public class SearchProductMetadataResponse
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}