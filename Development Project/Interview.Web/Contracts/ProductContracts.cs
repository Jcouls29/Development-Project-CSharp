using System;
using System.Collections.Generic;

namespace Interview.Web.Contracts
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ProductImageUris { get; set; } = new List<string>();
        public List<string> ValidSkus { get; set; } = new List<string>();
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<int> CategoryIds { get; set; } = new List<int>();
    }

    public class UpdateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ProductImageUris { get; set; } = new List<string>();
        public List<string> ValidSkus { get; set; } = new List<string>();
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<int> CategoryIds { get; set; } = new List<int>();
    }

    public class SearchProductsRequest
    {
        public string NameContains { get; set; }
        public string DescriptionContains { get; set; }
        public List<string> Skus { get; set; } = new List<string>();
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<int> CategoryIds { get; set; } = new List<int>();
        public bool MatchAllCategories { get; set; }
        public bool IncludeDescendantCategories { get; set; } = true;
    }

    public class ProductResponse
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ProductImageUris { get; set; } = new List<string>();
        public List<string> ValidSkus { get; set; } = new List<string>();
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<int> CategoryIds { get; set; } = new List<int>();
        public DateTime CreatedTimestamp { get; set; }
    }
}
