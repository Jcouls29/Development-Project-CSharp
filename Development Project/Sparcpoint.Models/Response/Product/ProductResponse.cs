using Sparcpoint.Models.Response.Category;
using System;
using System.Collections.Generic;

namespace Sparcpoint.Models.Response.Product
{
    public class ProductResponse
    {
        public DateTime CreatedTimestamp { get; set; }
        public string Description { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public List<CategoryResponse> Categories { get; set; }
    }
}