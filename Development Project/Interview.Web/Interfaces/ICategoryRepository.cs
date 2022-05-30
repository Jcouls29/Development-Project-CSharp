using Interview.Web.Models;
using System.Collections.Generic;

namespace Interview.Web.Interfaces
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        Category Add(Category category);
    }
}
