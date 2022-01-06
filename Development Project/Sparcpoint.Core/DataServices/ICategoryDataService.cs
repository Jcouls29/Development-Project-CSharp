using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.DataServices
{
    public interface ICategoryDataService
    {
        Task<List<Category>> GetCategories();
        Task CreateCategoryAsync(Category newProduct);
    }
}
