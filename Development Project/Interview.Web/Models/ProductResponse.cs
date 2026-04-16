using System;
using System.Collections.Generic;
using System.Linq;

namespace Interview.Web.Models
{
    public class ProductResponse
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public List<int> CategoryIds { get; set; }

        public static ProductResponse FromProduct(Sparcpoint.Product product)
        {
            return new ProductResponse
            {
                InstanceId = product.InstanceId,
                Name = product.Name,
                Description = product.Description,
                ProductImageUris = product.ProductImageUris,
                ValidSkus = product.ValidSkus,
                CreatedTimestamp = product.CreatedTimestamp,
                Metadata = product.Metadata?.ToDictionary(m => m.Key, m => m.Value) ?? new Dictionary<string, string>(),
                CategoryIds = product.CategoryIds ?? new List<int>()
            };
        }
    }
}
