using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;

namespace Interview.Web.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllProducts();
        Task<ProductResponseDto> AddProduct(ProductCreateDto dto);
        Task<IEnumerable<ProductResponseDto>> Search(string? query, string? category, string? metaKey, string? metaValue);
    }
}
