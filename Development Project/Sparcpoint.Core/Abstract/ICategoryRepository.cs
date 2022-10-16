using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract
{
    public interface ICategoryRepository
    {
        Task<int> AddCategoryAsync(Category category);
    }
}
