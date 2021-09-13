namespace Interview.Web.models
{
    /// <summary>
    /// class for product model
    /// </summary>
    public class ProductModel : Interview.Web.Models.BaseModel
    {
        /// <summary>
        /// product id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// product name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// product description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// product image Uri's
        /// </summary>
        public string ImageUris { get; set; }

        /// <summary>
        /// product color
        /// </summary>

        public string Color { get; set; }

        /// <summary>
        /// valid sku's
        /// </summary>
        public string ValidSkus { get; set; }

        /// <summary>
        /// to know product is active or discontinued
        /// </summary>
        public Boolean Active { get; set; }
    }
}
