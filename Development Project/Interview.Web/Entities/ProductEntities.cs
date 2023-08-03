using System.Collections.Generic;

namespace Interview.Web.Entities
{
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    
    // Navigation property for Categories
    public List<Category> Categories { get; set; } = new List<Category>();
    
    // Navigation property for Metadata
    public List<Metadata> Metadata { get; set; } = new List<Metadata>();
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Navigation property for Products
    public List<Product> Products { get; set; } = new List<Product>();
}

public class Metadata
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    
    // Navigation property for Products
    public List<Product> Products { get; set; } = new List<Product>();
}
}