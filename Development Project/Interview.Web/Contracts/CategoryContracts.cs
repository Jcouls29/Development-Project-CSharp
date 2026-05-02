using System;
using System.Collections.Generic;

namespace Interview.Web.Contracts
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<int> ParentCategoryIds { get; set; } = new List<int>();
    }

    public class CategoryResponse
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<int> ParentCategoryIds { get; set; } = new List<int>();
        public DateTime CreatedTimestamp { get; set; }
    }
}
