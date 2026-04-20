using Interview.Web.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Contracts
{
    /** IProductAppService defines the contract for product-related operations in the application layer.
    */
    public interface IProductAppService
    {
        /** Adds a new product to the system.
         * @param createProductDto The DTO containing the details of the product to be created.
         * @returns The instance ID of the newly created product.
         */
        Task<int> AddProduct(CreateProductDto createProductDto);

        /** Retrieves products based on a filter string.
         * @param filter A string containing the filter criteria in the format "Key:Value,Key:Value".
         * @returns A list of GetProductDto matching the filter criteria.
         */
        Task<IEnumerable<GetProductDto>> GetProduct(string filter);
    }
}
