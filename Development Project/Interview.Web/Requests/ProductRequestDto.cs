using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Requests
{
    /// <summary>
    /// Represents the new Product Request
    /// </summary>
    public class ProductRequestDto
    {

        /// <summary>
        /// Name of Product
        /// </summary>
         [Required(ErrorMessage = "Name of the Prodct is required, please provide")]
        public string Name { get; set; }

        /// <summary>
        /// Descibes the product
        /// </summary>
         [Required(ErrorMessage = "Description of the Prodct is required, please provide")]
        public string Description { get; set; }

        /// <summary>
        /// Describes the bar code id
        /// </summary>

        [Required(ErrorMessage = "SKU  of the Prodct is required, please provide")]
        public string StockKeepUnitCode { get; set; }

        /// <summary>
        /// Identity of Product Category
        /// </summary>
        [Required(ErrorMessage = "Category  of the Prodct is required, please provide")] 
        public int CategoryId { get; set; }

        /// <summary>
        /// Default Constructor 
        /// </summary>
        /// <returns></returns>
        public ProductRequestDto()
        { 

        }
    }
}
