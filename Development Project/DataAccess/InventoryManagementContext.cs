using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class InventoryManagementContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseInMemoryDatabase("InventoryDb");
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Metadata> Metadatas { get; set; }
}