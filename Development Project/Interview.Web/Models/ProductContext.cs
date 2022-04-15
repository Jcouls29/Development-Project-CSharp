using Microsoft.EntityFrameworkCore;
namespace Interview.Web.Models;

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    //EVAL: Returns set of products
    public DbSet<Product> Products
    {
        get; set;
    }
}