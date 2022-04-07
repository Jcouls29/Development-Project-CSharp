using System;
using System.Collections.Generic;
using System.Text;

namespace SparcpointServices.Models
{
   public class ProductModel
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public List<ProductAttributeModel> ProductAttributeModel { get; set; }
        public List<ProductCategoryModel> ProductCategoryModel { get; set; }
        public int Inventory { get; set; }
    }
}
