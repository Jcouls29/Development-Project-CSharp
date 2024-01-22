using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace Sparcpoint.Models.Tables
{
    [Table("[Instances].[Products]")]
    public class Product
    {
        [Key]
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ValidSkus { get; set; }
        public string ProductImageUris { get; set; }

        [Computed]
        public DateTime CreatedTimestamp { get; set; }
        [Computed]
        public List<ProductCategory> Categories { get; set; }
        [Computed]
        public List<ProductAttribute> Attributes { get; set; }
    }
}