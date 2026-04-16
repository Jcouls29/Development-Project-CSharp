using Sparcpoint.Models;
using System;
using System.Collections.Generic;

namespace Sparcpoint.Features.Products.Queries.GetProducts
{
    public class ProductsGetResponseItem
    {
        public int InstanceId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ProductImageUris { get; set; }

        public string ValidSkus { get; set; }

        public DateTime CreatedTimestamp { get; set; }

        public List<ProductsGetCategoryItem> Categories { get; set; } = new List<ProductsGetCategoryItem>();

        public List<AttributeItem> Attributes { get; set; } = new List<AttributeItem>();
    }

    public class ProductsGetCategoryItem
    {
        public int InstanceId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
