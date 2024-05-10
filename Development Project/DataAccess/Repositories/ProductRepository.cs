using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class ProductRepository : IProductRepository
{
    public ProductRepository(InventoryManagementContext inventoryManagementContext)
    {
        _inventoryManagementContext = inventoryManagementContext;
    }

    private readonly InventoryManagementContext _inventoryManagementContext;
    
    public async Task<List<Product>> GetProducts()
    {
        return await _inventoryManagementContext.Products
            .Include(p => p.Categories)
            .Include(p => p.MetadataAttributes)
            .ToListAsync();
    }

    public async Task<List<Product>> GetProductsWithMetadata(List<Metadata> metadatas)
    {
        return await _inventoryManagementContext.Products
            .Include(p => p.Categories)
            .Include(p => p.MetadataAttributes)
            .Where(product => product.MetadataAttributes.Any(metaData => metadatas.Contains(metaData)))
            .ToListAsync();
    }
    
    public async Task<Product> AddProduct(Product product)
    {
        await _inventoryManagementContext.Products.AddAsync(product);
        await _inventoryManagementContext.SaveChangesAsync();
        return await _inventoryManagementContext.Products
            .Include(p => p.Categories)
            .Include(p => p.MetadataAttributes)
            .SingleAsync(p => p.Id == product.Id);
    }

    public async Task<List<Product>> AddProducts(List<Product> products)
    {
        var productIds = products.Select(product => product.Id).ToList();
        await _inventoryManagementContext.Products.AddRangeAsync(products);
        await _inventoryManagementContext.SaveChangesAsync();
        return await _inventoryManagementContext.Products
            .Include(p => p.Categories)
            .Include(p => p.MetadataAttributes)
            .Where(product => productIds.Contains(product.Id))
            .ToListAsync();
    }
    
    //EVAL: Some of the requirements are unclear to me, for instance "Goals" point 4 lists that we want to be able to
    // "add and remove products from inventory". However, "Requirements" point 1 states that
    // "products can be added to the system, but never deleted". It's not entirely clear if products should be able to
    // be removed/deleted or not. I would ask a product manager, or whoever is responsible for business requirements,
    // clarifying questions before continuing with implementation of these portions.
}