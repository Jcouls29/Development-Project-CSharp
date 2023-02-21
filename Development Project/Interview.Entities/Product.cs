using System;
using System.Collections.Generic;

namespace Interview.Entities
{
    public class Product
    {
        Product()
        {
            ProductAttributes = new List<ProductAttributes>();
            ProductCategories = new List<ProductCategories>();
        }

        /// <summary>
        /// Instance Id
        /// </summary>
        public int InstanceId { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Product Image Uris
        /// </summary>
        public string ProductImageUris { get; set; }
        /// <summary>
        /// Valid Skus
        /// </summary>
        public string ValidSkus { get; set; }
        /// <summary>
        /// Created Timestamp
        /// </summary>
        public DateTime CreatedTimestamp { get; set; }
        /// <summary>
        /// Product Attributes
        /// </summary>
        public List<ProductAttributes> ProductAttributes { get; set; }
        /// <summary>
        /// Product Categories
        /// </summary>
        public List<ProductCategories> ProductCategories { get; set; }
    }
}