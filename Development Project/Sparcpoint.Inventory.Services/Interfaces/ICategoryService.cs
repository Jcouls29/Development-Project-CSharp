using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Inventory.Models.DTOs.Requests;
using Sparcpoint.Inventory.Models.DTOs.Responses;

namespace Sparcpoint.Inventory.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);
        Task<CategoryResponse?> GetCategoryByIdAsync(int instanceId);
        Task<List<CategoryResponse>> GetAllCategoriesAsync();
    }
}