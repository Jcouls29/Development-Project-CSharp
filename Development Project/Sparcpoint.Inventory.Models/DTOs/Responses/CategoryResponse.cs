using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models.DTOs.Responses
{
    public class CategoryResponse
    {
        public int InstanceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<int> ParentCategoryIds { get; set; } = new List<int>();
        public DateTime CreatedTimestamp { get; set; }
    }
}