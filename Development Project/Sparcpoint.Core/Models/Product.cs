using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public List<ProductAttributes> ProductAttributes { get; set; }
    }

    public class ProductAttributes
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
