using System.Collections.Generic;

namespace Sparcpoint.Models
{
    public class ProductDetail : Product
    {
        public Dictionary<string, string> Attributes { get; set; }
        public List<ProductCategory> Categories { get; set; }
    }
}
