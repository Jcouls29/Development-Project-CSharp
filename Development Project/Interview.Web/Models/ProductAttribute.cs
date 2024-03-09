namespace Interview.Web.Models
{
    /// <summary>
    /// Represents an attribute associated with a product.
    /// </summary>
    public class ProductAttribute
    {
        /// <summary>
        /// Gets or sets the unique identifier of the product instance.
        /// </summary>
        public int InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the key of the product attribute.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value of the product attribute.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the product associated with this attribute.
        /// </summary>
        public Product Product { get; set; }
    }
}
