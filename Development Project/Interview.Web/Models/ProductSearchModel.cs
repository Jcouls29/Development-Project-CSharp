namespace Interview.Web.Models
{
    /// <summary>
    /// Represents the search criteria for querying products.
    /// </summary>
    public class ProductSearchModel
    {
        /// <summary>
        /// Gets or sets the Stock Keeping Unit (SKU) of the product.
        /// </summary>
        public string SKU { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the category to which the product belongs.
        /// </summary>
        public string CategoryName { get; set; }
    }
}
