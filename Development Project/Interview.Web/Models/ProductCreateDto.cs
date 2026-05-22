using System.Collections.Generic;

namespace Interview.Web.Models
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public List<string> Categories { get; set; } = new List<string>();

        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
