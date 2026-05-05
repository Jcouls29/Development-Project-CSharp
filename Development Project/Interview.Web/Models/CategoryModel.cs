using System;
using System.Collections.Generic;

namespace Interview.Web.Models;

public class CategoryModel
{
    public DateTime CreatedTimestamp { get; set; }
    public string Description { get; set; }
    public int InstanceId { get; set; }
    public string Name { get; set; }
    
    private List<ProductModel>? _products;
    
    public List<ProductModel>? Products
    {
        get => _products;
    }

    public void HydrateProducts()
    {
        if (_products != null)
        {
            return;
        }
        
        // EVAL: This will populate the Products property with ProductModel entities via the
        //       ProductCategories relation.
    }
}