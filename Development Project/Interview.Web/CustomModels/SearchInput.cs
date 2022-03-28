using System.ComponentModel.DataAnnotations;

namespace Interview.Web.CustomModels
{
    //This  is the request body for searching products
    public class SearchInput
    {
        /// <summary>
        /// Name of the product
        /// </summary>
        [Required(ErrorMessage ="Product Name is required")]
        public string ProductName { get; set; }
        /// <summary>
        /// Product description which can have any details about project
        /// </summary>
        public string ProductDescription { get; set; }
        /// <summary>
        /// Category name that product belongs to
        /// </summary>
        public string ProductCategory { get; set; }
        /// <summary>
        /// Key attribute associated for the product
        /// </summary>
        public string ProductKey { get; set; }
    }
}
