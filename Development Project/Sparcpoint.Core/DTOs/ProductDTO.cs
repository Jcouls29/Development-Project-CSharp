using System.Collections.Generic;
using Sparcpoint.Models;

namespace Sparcpoint.DTOs
{
    public class ProductDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<string> ProductImageUris { get; set; }
        public IList<string> ValidSkus { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public IEnumerable<ProductMetadata> Metadata { get; set; }
    }
}
