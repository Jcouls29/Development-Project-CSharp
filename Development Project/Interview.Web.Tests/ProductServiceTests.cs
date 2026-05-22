using Interview.Web.Data;
using Interview.Web.Models;
using Interview.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Tests;

public class ProductServiceTests
{
    private static ApplicationDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task GetAllProducts_EmptyDatabase_ReturnsEmptyList()
    {
        await using var db = CreateContext(nameof(GetAllProducts_EmptyDatabase_ReturnsEmptyList));
        var service = new ProductService(db);

        var result = await service.GetAllProducts();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsMappedProductFields()
    {
        var productId = Guid.NewGuid();
        await using var db = CreateContext(nameof(GetAllProducts_ReturnsMappedProductFields));

        db.Products.Add(new Product
        {
            Id = productId,
            Name = "Widget",
            Description = "A test widget",
            Price = 19.99m,
            Metadata = new Dictionary<string, string> { ["color"] = "blue" }
        });
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        var result = (await service.GetAllProducts()).ToList();

        Assert.Single(result);
        var product = result[0];
        Assert.Equal(productId, product.Id);
        Assert.Equal("Widget", product.Name);
        Assert.Equal("A test widget", product.Description);
        Assert.Equal(19.99m, product.Price);
        Assert.Equal("blue", product.Metadata["color"]);
        Assert.Empty(product.Categories);
    }

    [Fact]
    public async Task GetAllProducts_IncludesCategoryNames()
    {
        var productId = Guid.NewGuid();
        await using var db = CreateContext(nameof(GetAllProducts_IncludesCategoryNames));

        var category = new Category { Name = "Electronics" };
        var product = new Product
        {
            Id = productId,
            Name = "Phone",
            Description = "Smartphone",
            Price = 999m
        };
        product.ProductCategories.Add(new ProductCategory
        {
            Product = product,
            Category = category
        });

        db.Products.Add(product);
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        var result = (await service.GetAllProducts()).Single();

        Assert.Equal(productId, result.Id);
        Assert.Single(result.Categories);
        Assert.Equal("Electronics", result.Categories[0]);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsMultipleProducts()
    {
        await using var db = CreateContext(nameof(GetAllProducts_ReturnsMultipleProducts));

        db.Products.AddRange(
            new Product { Name = "Alpha", Description = "First", Price = 1m },
            new Product { Name = "Beta", Description = "Second", Price = 2m });
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        var result = (await service.GetAllProducts()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "Alpha");
        Assert.Contains(result, p => p.Name == "Beta");
    }

    [Fact]
    public async Task GetAllProducts_ProductWithMultipleCategories_ReturnsAllCategoryNames()
    {
        var productId = Guid.NewGuid();
        await using var db = CreateContext(nameof(GetAllProducts_ProductWithMultipleCategories_ReturnsAllCategoryNames));

        var electronics = new Category { Name = "Electronics" };
        var sale = new Category { Name = "Sale" };
        var product = new Product
        {
            Id = productId,
            Name = "Tablet",
            Description = "Device",
            Price = 499m
        };
        product.ProductCategories.Add(new ProductCategory { Product = product, Category = electronics });
        product.ProductCategories.Add(new ProductCategory { Product = product, Category = sale });

        db.Products.Add(product);
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        var result = (await service.GetAllProducts()).Single();

        Assert.Equal(2, result.Categories.Count);
        Assert.Contains("Electronics", result.Categories);
        Assert.Contains("Sale", result.Categories);
    }

    [Fact]
    public async Task AddProduct_PersistsProductWithMappedFields()
    {
        await using var db = CreateContext(nameof(AddProduct_PersistsProductWithMappedFields));
        var service = new ProductService(db);

        var dto = new ProductCreateDto
        {
            Name = "Gadget",
            Description = "Useful gadget",
            Price = 29.99m,
            Metadata = new Dictionary<string, string> { ["size"] = "large" }
        };

        var result = await service.AddProduct(dto);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Gadget", result.Name);
        Assert.Equal("Useful gadget", result.Description);
        Assert.Equal(29.99m, result.Price);
        Assert.Equal("large", result.Metadata["size"]);
        Assert.Empty(result.Categories);

        var persisted = await db.Products.FindAsync(result.Id);
        Assert.NotNull(persisted);
        Assert.Equal("Gadget", persisted.Name);
    }

    [Fact]
    public async Task AddProduct_WithCategories_ReturnsCategoryNames()
    {
        await using var db = CreateContext(nameof(AddProduct_WithCategories_ReturnsCategoryNames));
        var service = new ProductService(db);

        var dto = new ProductCreateDto
        {
            Name = "Laptop",
            Description = "Notebook",
            Price = 1200m,
            Categories = new List<string> { "Electronics", "Computers" }
        };

        var result = await service.AddProduct(dto);

        Assert.Equal(2, result.Categories.Count);
        Assert.Contains("Electronics", result.Categories);
        Assert.Contains("Computers", result.Categories);
        Assert.Equal(2, await db.Categories.CountAsync());
    }

    [Fact]
    public async Task AddProduct_ReusesExistingCategory()
    {
        await using var db = CreateContext(nameof(AddProduct_ReusesExistingCategory));
        db.Categories.Add(new Category { Name = "Electronics" });
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        await service.AddProduct(new ProductCreateDto
        {
            Name = "Phone",
            Description = "Mobile",
            Price = 800m,
            Categories = new List<string> { "Electronics" }
        });

        var result = await service.AddProduct(new ProductCreateDto
        {
            Name = "Tablet",
            Description = "Device",
            Price = 500m,
            Categories = new List<string> { "Electronics" }
        });

        Assert.Single(result.Categories);
        Assert.Equal("Electronics", result.Categories[0]);
        Assert.Equal(1, await db.Categories.CountAsync());
        Assert.Equal(2, await db.ProductCategories.CountAsync());
    }

    [Fact]
    public async Task AddProduct_NullMetadata_UsesEmptyDictionary()
    {
        await using var db = CreateContext(nameof(AddProduct_NullMetadata_UsesEmptyDictionary));
        var service = new ProductService(db);

        var result = await service.AddProduct(new ProductCreateDto
        {
            Name = "Simple",
            Description = "No metadata",
            Price = 5m,
            Metadata = null
        });

        Assert.NotNull(result.Metadata);
        Assert.Empty(result.Metadata);
    }

    [Fact]
    public async Task Search_NoFilters_ReturnsAllProducts()
    {
        await using var db = CreateContext(nameof(Search_NoFilters_ReturnsAllProducts));
        db.Products.AddRange(
            new Product { Name = "Alpha", Description = "First", Price = 1m },
            new Product { Name = "Beta", Description = "Second", Price = 2m });
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        var result = (await service.Search(null, null, null, null)).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Search_ByQuery_MatchesName()
    {
        await using var db = CreateContext(nameof(Search_ByQuery_MatchesName));
        db.Products.AddRange(
            new Product { Name = "Red Widget", Description = "A", Price = 1m },
            new Product { Name = "Blue Gadget", Description = "B", Price = 2m });
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        var result = (await service.Search("Widget", null, null, null)).ToList();

        Assert.Single(result);
        Assert.Equal("Red Widget", result[0].Name);
    }

    [Fact]
    public async Task Search_ByQuery_MatchesDescription()
    {
        await using var db = CreateContext(nameof(Search_ByQuery_MatchesDescription));
        db.Products.AddRange(
            new Product { Name = "A", Description = "waterproof case", Price = 1m },
            new Product { Name = "B", Description = "basic cover", Price = 2m });
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        var result = (await service.Search("waterproof", null, null, null)).ToList();

        Assert.Single(result);
        Assert.Equal("A", result[0].Name);
    }

    [Fact]
    public async Task Search_ByCategory_FiltersCorrectly()
    {
        await using var db = CreateContext(nameof(Search_ByCategory_FiltersCorrectly));

        var electronics = new Category { Name = "Electronics" };
        var home = new Category { Name = "Home" };
        var phone = new Product { Name = "Phone", Description = "Mobile", Price = 800m };
        var lamp = new Product { Name = "Lamp", Description = "Light", Price = 40m };
        phone.ProductCategories.Add(new ProductCategory { Product = phone, Category = electronics });
        lamp.ProductCategories.Add(new ProductCategory { Product = lamp, Category = home });

        db.Products.AddRange(phone, lamp);
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        var result = (await service.Search(null, "Electronics", null, null)).ToList();

        Assert.Single(result);
        Assert.Equal("Phone", result[0].Name);
        Assert.Contains("Electronics", result[0].Categories);
    }

    [Fact]
    public async Task Search_ByMetadataKeyValue_FiltersCorrectly()
    {
        await using var db = CreateContext(nameof(Search_ByMetadataKeyValue_FiltersCorrectly));
        db.Products.AddRange(
            new Product
            {
                Name = "Blue Item",
                Description = "A",
                Price = 1m,
                Metadata = new Dictionary<string, string> { ["color"] = "blue" }
            },
            new Product
            {
                Name = "Red Item",
                Description = "B",
                Price = 2m,
                Metadata = new Dictionary<string, string> { ["color"] = "red" }
            });
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        var result = (await service.Search(null, null, "color", "blue")).ToList();

        Assert.Single(result);
        Assert.Equal("Blue Item", result[0].Name);
    }

    [Fact]
    public async Task Search_CombinedFilters_ReturnsMatchingProduct()
    {
        await using var db = CreateContext(nameof(Search_CombinedFilters_ReturnsMatchingProduct));

        var category = new Category { Name = "Electronics" };
        var match = new Product
        {
            Name = "Pro Phone",
            Description = "flagship smartphone",
            Price = 999m,
            Metadata = new Dictionary<string, string> { ["tier"] = "pro" }
        };
        var other = new Product
        {
            Name = "Pro Case",
            Description = "flagship cover",
            Price = 49m,
            Metadata = new Dictionary<string, string> { ["tier"] = "basic" }
        };
        match.ProductCategories.Add(new ProductCategory { Product = match, Category = category });
        other.ProductCategories.Add(new ProductCategory { Product = other, Category = category });

        db.Products.AddRange(match, other);
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        var result = (await service.Search("Phone", "Electronics", "tier", "pro")).ToList();

        Assert.Single(result);
        Assert.Equal("Pro Phone", result[0].Name);
    }

    [Fact]
    public async Task Search_NoMatches_ReturnsEmpty()
    {
        await using var db = CreateContext(nameof(Search_NoMatches_ReturnsEmpty));
        db.Products.Add(new Product { Name = "Widget", Description = "Test", Price = 1m });
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        var result = await service.Search("nonexistent", null, null, null);

        Assert.Empty(result);
    }
}
