using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sparcpoint.Models.Products
{
    public class AddProduct
    {
        [Required,MaxLength(256)]
        public string Name { get; set; }
        [Required, MaxLength(256)]
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public List<ProductAttributes> ProductMetadata { get; set; }
    }
}
