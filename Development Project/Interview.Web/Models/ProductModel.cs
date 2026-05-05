using System;
using System.Collections.Generic;

namespace Interview.Web.Models;

#region DTOs

public class CreateProductDto
{
    public string Description { get; set; }
    public string Name { get; set; }
    public string ProductImageUris { get; set; }
    public string ValidSkus { get; set; }
}

#endregion

public class ProductModel
{
    public DateTime CreatedTimestamp { get; set; }
    public string Description { get; set; }
    public int InstanceId { get; set; }
    public string Name { get; set; }
    public string ProductImageUris { get; set; }
    public string ValidSkus { get; set; }
    
    private List<CategoryModel>? _categories = null;

    public List<CategoryModel>? Categories
    {
        get => _categories;
    }

    public void HydrateCategories()
    {
        if (_categories != null)
        {
            return;
        }
        
        // EVAL: This will populate the Categories property with CategoryModel entities via the
        //       ProductCategories relation.
    }
}