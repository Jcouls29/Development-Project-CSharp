using System.Collections.Generic;

namespace Interview.Web.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
