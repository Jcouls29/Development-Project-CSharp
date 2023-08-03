using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Models
{
    // Product.cs
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public List<Category> Categories { get; set; }
    public List<Metadata> Metadata { get; set; }
}

// Category.cs
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// Metadata.cs
public class Metadata
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}
}
