using Interview.Web.Models;
using Interview.Web.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public Task<Category> AddAsync(Category category)
        {
            category.Id = category.Id == Guid.Empty ? Guid.NewGuid() : category.Id;
            category.CreatedAt = DateTime.UtcNow;
            return _repo.AddAsync(category);
        }

        public Task<IEnumerable<Category>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Category> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        public Task<IEnumerable<Category>> GetChildrenAsync(Guid parentId) => _repo.GetChildrenAsync(parentId);

        public Task<IEnumerable<Category>> GetHierarchyAsync() => _repo.GetHierarchyAsync();
    }
}
