namespace Interview.Web.Models
{
    /// <summary>
    /// Represents the model used for creating a new product.
    /// </summary>
    public class CreateProductModel
    {
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
        /// Gets or sets the name of the category to which the product belongs.
        /// </summary>
        public string CategoryName { get; set; }
    }
}