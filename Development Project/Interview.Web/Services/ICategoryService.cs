using Interview.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    public interface ICategoryService
    {
        Task<Category> AddAsync(Category category);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(Guid id);
        Task<IEnumerable<Category>> GetChildrenAsync(Guid parentId);
        Task<IEnumerable<Category>> GetHierarchyAsync();
    }
}
