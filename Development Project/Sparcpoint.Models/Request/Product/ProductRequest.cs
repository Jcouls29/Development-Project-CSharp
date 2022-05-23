using System.Collections.Generic;

namespace Sparcpoint.Models.Request.Product
{
    public class ProductRequest
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public List<int> Categories { get; set; }
        public string RequestedBy { get; set; }
    }
}