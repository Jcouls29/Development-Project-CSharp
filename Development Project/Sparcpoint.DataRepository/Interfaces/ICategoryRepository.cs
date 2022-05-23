using Sparcpoint.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.DataRepository.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAll();

        Task<List<Category>> GetProductCategories(int productId);
    }
}