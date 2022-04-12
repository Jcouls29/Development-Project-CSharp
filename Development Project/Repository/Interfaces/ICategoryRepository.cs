using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Repository.Interfaces
{
    public interface ICategoryRepository
    {

        IEnumerable<Category> GetCategories();
        Category GetCategoryId(int categoryId);

    }
}