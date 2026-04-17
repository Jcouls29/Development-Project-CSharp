using System.Collections.Generic;

namespace Sparcpoint.Core.Models
{
    public class ProductSearchDto
    {
        public string Name { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public List<string> Categories { get; set; }
    }
}
