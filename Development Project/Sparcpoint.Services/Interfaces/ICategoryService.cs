using Sparcpoint.Models.Request.Product;
using Sparcpoint.Models.Response.Category;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Models.Request.Category;

namespace Sparcpoint.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<GetCategoryResponse> GetAllCategories(GetCategoryRequest request);

        Task<List<CategoryResponse>> GetProductCategories(GetProductRequestById request);
    }
}