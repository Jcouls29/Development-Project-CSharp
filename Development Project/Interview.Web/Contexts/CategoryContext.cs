using Interview.Web.Models;
using System.Collections.Generic;

namespace Interview.Web.Contexts;

#region DTOs

public class CreateCategoryDto
{
    public string Description { get; set; }
    public string Name { get; set; }
}

#endregion

public class CategoryContext
{
    public static CategoryModel CreateCategory(CreateCategoryDto category)
    {
        var sql = @"INSERT INTO Categories
                        ([Description], [Name])
                   VALUES
                        (@Description, @Name)
                   ";
        // TODO: Exectue SQL ~ var newCat = ...
        return new CategoryModel();
    }
    
    public static List<CategoryModel> GetCategories()
    {
        var sql = "SELECT * FROM Categories";
        // TODO: Execute SQL ~ var categories = ...
        return new List<CategoryModel>();
    }
}