using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparcpoint.Models.DTOs;

namespace Sparcpoint.Models
{
    public class Product
    {
        public int InstanceId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<string> ProductImageUris { get; private set; } = new List<string>();
        public List<string> ValidSkus { get; private set; } = new List<string>();
        public List<Category> Categories { get; private set; } = new List<Category>();
        public Dictionary<string, string> Metadata { get; private set; } = new Dictionary<string, string>();

        public static Product Create(ProductDto product)
        {
            return new Product
            {
                Name = product.Name,
                Description = product.Description,
                ProductImageUris = product.ProductImageUris,
                Categories = product.Categories?.Select(Category.Create).ToList(),
                Metadata = product.Metadata,
            };
        }
    }
}
