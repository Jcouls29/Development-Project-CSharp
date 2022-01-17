using System;
using System.Collections.Generic;

namespace SparcPoint.Inventory.DataModels
{
    public class Product
    {
        public Product()
        {
            //Populate Category and Product Attributes.
        }

        public int InstanceId { get; set; }
        public string Name { get; set; }
        
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public string CreatedTimeStamp { get; set; }


        /// <summary>
        /// better way to do this is in Entity framework with foreigh key relationships via ProductCategory table
        /// </summary>
        public Category Category { get; set; }

        public List<IAttribute> ProductAttributes { get; set; }
    }
}
