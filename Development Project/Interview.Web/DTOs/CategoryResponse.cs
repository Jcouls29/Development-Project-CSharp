using System;
using System.Collections.Generic;
using Sparcpoint.Inventory.Models;

namespace Interview.Web.DTOs
{
    /// <summary>
    /// EVAL: Response DTO for category data.
    /// Includes parent category IDs to represent the hierarchy.
    /// </summary>
    public class CategoryResponse
    {
        public int InstanceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedTimestamp { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new();
        public List<int> ParentCategoryIds { get; set; } = new();

        public static CategoryResponse FromCategory(Category category)
        {
            return new CategoryResponse
            {
                InstanceId = category.InstanceId,
                Name = category.Name,
                Description = category.Description,
                CreatedTimestamp = category.CreatedTimestamp,
                Attributes = category.Attributes ?? new Dictionary<string, string>(),
                ParentCategoryIds = category.ParentCategoryIds ?? new List<int>()
            };
        }
    }
}
