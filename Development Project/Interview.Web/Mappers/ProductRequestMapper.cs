using Application.ProductFeatures.Commands;
using Interview.Web.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Mappers
{
    /// <summary>
    /// Mapper class to map product related requests to commands
    /// </summary>
    public static  class ProductRequestMapper
    {
        /// <summary>
        /// Maps the incoming request to new product command
        /// </summary>
        /// <param name="newProduct"></param>
        /// <returns></returns>
        public static CreateProductCommand MapProductRequestToCommand(ProductRequestDto newProduct)
        {
            return new CreateProductCommand()
            {
                CategoryId = newProduct.CategoryId,
                Description = newProduct.Description,
                Name = newProduct.Name,
                StockKeepUnitCode = newProduct.StockKeepUnitCode
            };
        }
        /// <summary>
        /// Maps in the incoming product and quantity to inventory command
        /// </summary>
        /// <param name="product_quantity"></param>
        /// <returns></returns>
        public static AddProductsToInventoryCommand MapInventoryRequestToCommand(Dictionary<int, int> product_quantity)
        {
            return new AddProductsToInventoryCommand()
            {
                Product_Quantity = product_quantity
            };
        }
    }
}
