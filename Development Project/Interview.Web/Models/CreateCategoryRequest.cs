using System.Collections.Generic;

namespace Interview.Web.Models
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CategoryAttributeRequest> CategoryAttributes { get; set; }
        public List<CategoryOfCategoryRequest> Categories { get; set; }
    }
}
