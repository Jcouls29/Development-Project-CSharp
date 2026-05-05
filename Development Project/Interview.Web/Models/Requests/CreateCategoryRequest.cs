using System.Collections.Generic;

namespace Interview.Web.Models.Requests
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public int[] ParentCategoryIds { get; set; }
    }
}
