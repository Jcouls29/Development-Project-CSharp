using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sparcpoint.Models.DTOs;
using Sparcpoint.Repositories;

namespace Sparcpoint.Services
{
    public interface ICategoryService
    {
        Task CreateCategory(CategoryDto cagegory);
        Task FindByIdAsync(int id);
    }
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService() { }
        public Task CreateCategory(CategoryDto cagegory)
        {
            throw new NotImplementedException();
        }

        public Task FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
