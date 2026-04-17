using System.Threading.Tasks;
using Sparcpoint.Core.Models;

namespace Sparcpoint.Application.Repositories
{
    public interface IProductRepository
    {
        // Creates a product instance in the Instances.Products table and returns the generated InstanceId (int)
        Task<int> CreateProductAsync(ProductCreateDto dto);

        // Search products by optional name, metadata key/value pairs (all must match), and categories (match any)
        Task<System.Collections.Generic.IEnumerable<ProductResponseDto>> SearchAsync(
            string name,
            System.Collections.Generic.Dictionary<string, string> metadata = null,
            System.Collections.Generic.List<string> categories = null);
    }
}
