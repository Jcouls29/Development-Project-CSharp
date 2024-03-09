using System;

namespace Interview.Web.Models
{
    /// <summary>
    /// Represents a product entity.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Gets or sets the unique identifier of the product instance.
        /// </summary>
        public int InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the URIs of the product images.
        /// </summary>
        public string ProductImageUris { get; set; }

        /// <summary>
        /// Gets or sets the valid SKUs (Stock Keeping Units) of the product.
        /// </summary>
        public string ValidSkus { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the product was created.
        /// </summary>
        public DateTime CreatedTimestamp { get; set; }
    }
}
