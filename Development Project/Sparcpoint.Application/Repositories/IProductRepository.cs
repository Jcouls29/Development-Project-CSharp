using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Core.Models;

namespace Sparcpoint.Application.Repositories
{
    public interface IProductRepository
    {
        // Creates a product instance in the Instances.Products table and returns the generated InstanceId (int)
        Task<int> CreateProductAsync(ProductCreateDto dto);

        // Get a single product by InstanceId
        Task<ProductResponseDto> GetByIdAsync(int id);

        // Search products by optional name, metadata key/value pairs (all must match), and categories (match any)
        Task<IEnumerable<ProductResponseDto>> SearchAsync(
            string name,
            Dictionary<string, string> metadata = null,
            List<string> categories = null);
    }
}
