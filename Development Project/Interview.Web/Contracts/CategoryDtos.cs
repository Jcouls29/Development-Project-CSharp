using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Interview.Web.Contracts
{
    public sealed class CategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, string>? Attributes { get; set; }
        public int[]? ParentCategoryIds { get; set; }

        public Category ToModel(int instanceId = 0) => new()
        {
            InstanceId = instanceId,
            Name = Name,
            Description = Description,
            Attributes = Attributes?.Select(kv => new ProductAttribute(kv.Key, kv.Value)).ToArray() ?? Array.Empty<ProductAttribute>(),
            ParentCategoryIds = ParentCategoryIds ?? Array.Empty<int>(),
        };
    }
}
