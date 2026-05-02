using Interview.Web.Contracts;
using Interview.Web.Data;
using Interview.Web.Infrastructure;
using Interview.Web.Services;
using Xunit;

namespace Interview.Web.Tests;

public class InventoryManagementServiceTests
{
    [Fact]
    public async Task CreateProductAsync_TrimsAndDeduplicatesValuesBeforeSaving()
    {
        var repository = new RecordingInventoryRepository();
        var service = new InventoryManagementService(repository);

        await service.CreateProductAsync(new CreateProductRequest
        {
            Name = "Blue Hammer",
            Description = "16oz claw hammer",
            ProductImageUris = new List<string> { "https://example.test/hammer.png", "https://example.test/hammer.png" },
            ValidSkus = new List<string> { "HAM-001", "ham-001" },
            Attributes = new Dictionary<string, string>
            {
                ["color"] = "Blue",
                ["brand"] = "Acme"
            },
            CategoryIds = new List<int> { 1, 1, 2 }
        });

        Assert.NotNull(repository.SavedCreateProductRequest);
        Assert.Equal("Blue Hammer", repository.SavedCreateProductRequest!.Name);
        Assert.Equal("16oz claw hammer", repository.SavedCreateProductRequest.Description);
        Assert.Equal(new[] { "https://example.test/hammer.png" }, repository.SavedCreateProductRequest.ProductImageUris);
        Assert.Equal(new[] { "HAM-001" }, repository.SavedCreateProductRequest.ValidSkus);
        Assert.Equal("Blue", repository.SavedCreateProductRequest.Attributes["color"]);
        Assert.Equal("Acme", repository.SavedCreateProductRequest.Attributes["brand"]);
        Assert.Equal(new[] { 1, 2 }, repository.SavedCreateProductRequest.CategoryIds);
    }

    [Fact]
    public async Task AddInventoryAsync_AggregatesDuplicateItemsAndUsesDefaultTypeCategory()
    {
        var repository = new RecordingInventoryRepository();
        var service = new InventoryManagementService(repository);

        await service.AddInventoryAsync(new InventoryAdjustmentRequest
        {
            Items = new List<InventoryAdjustmentItemRequest>
            {
                new() { ProductId = 7, Quantity = 2 },
                new() { ProductId = 7, Quantity = 3 },
                new() { ProductId = 8, Quantity = 5 }
            }
        });

        Assert.NotNull(repository.SavedAddInventoryRequest);
        Assert.Equal("manual-add", repository.SavedAddInventoryRequest!.TypeCategory);
        Assert.Equal(2, repository.SavedAddInventoryRequest.Items.Count);
        Assert.Contains(repository.SavedAddInventoryRequest.Items, item => item.ProductId == 7 && item.Quantity == 5);
        Assert.Contains(repository.SavedAddInventoryRequest.Items, item => item.ProductId == 8 && item.Quantity == 5);
    }

    [Fact]
    public async Task RemoveInventoryAsync_RejectsNonPositiveQuantities()
    {
        var repository = new RecordingInventoryRepository();
        var service = new InventoryManagementService(repository);

        var exception = await Assert.ThrowsAsync<ApiValidationException>(() =>
            service.RemoveInventoryAsync(new InventoryAdjustmentRequest
            {
                Items = new List<InventoryAdjustmentItemRequest>
                {
                    new() { ProductId = 1, Quantity = 0 }
                }
            }));

        Assert.Equal("Inventory quantities must be greater than zero.", exception.Message);
    }

    [Fact]
    public async Task CreateCategoryAsync_RejectsDuplicateAttributeKeysIgnoringCase()
    {
        var repository = new RecordingInventoryRepository();
        var service = new InventoryManagementService(repository);

        var exception = await Assert.ThrowsAsync<ApiValidationException>(() =>
            service.CreateCategoryAsync(new CreateCategoryRequest
            {
                Name = "Hardware",
                Description = "General hardware",
                Attributes = new Dictionary<string, string>
                {
                    ["department"] = "Tools",
                    ["Department"] = "Merch"
                }
            }));

        Assert.Equal("Duplicate attribute key 'Department' is not allowed.", exception.Message);
    }

    [Fact]
    public async Task SearchProductsAsync_NormalizesFiltersAndPreservesCategoryFlags()
    {
        var repository = new RecordingInventoryRepository();
        var service = new InventoryManagementService(repository);

        await service.SearchProductsAsync(new SearchProductsRequest
        {
            NameContains = "Hammer",
            DescriptionContains = "claw tool",
            Skus = new List<string> { "HAM-001", "ham-001" },
            Attributes = new Dictionary<string, string>
            {
                ["brand"] = "Acme"
            },
            CategoryIds = new List<int> { 4, 4, 9 },
            MatchAllCategories = true,
            IncludeDescendantCategories = false
        });

        Assert.NotNull(repository.SavedSearchProductsRequest);
        Assert.Equal("Hammer", repository.SavedSearchProductsRequest!.NameContains);
        Assert.Equal("claw tool", repository.SavedSearchProductsRequest.DescriptionContains);
        Assert.Equal(new[] { "HAM-001" }, repository.SavedSearchProductsRequest.Skus);
        Assert.Equal("Acme", repository.SavedSearchProductsRequest.Attributes["brand"]);
        Assert.Equal(new[] { 4, 9 }, repository.SavedSearchProductsRequest.CategoryIds);
        Assert.True(repository.SavedSearchProductsRequest.MatchAllCategories);
        Assert.False(repository.SavedSearchProductsRequest.IncludeDescendantCategories);
    }

    [Fact]
    public async Task CreateCategoryAsync_TrimsAndDeduplicatesParentCategoryIdsBeforeSaving()
    {
        var repository = new RecordingInventoryRepository();
        var service = new InventoryManagementService(repository);

        await service.CreateCategoryAsync(new CreateCategoryRequest
        {
            Name = "Hammers",
            Description = "Hammer family",
            Attributes = new Dictionary<string, string>
            {
                ["aisle"] = "A1"
            },
            ParentCategoryIds = new List<int> { 2, 2, 7 }
        });

        Assert.NotNull(repository.SavedCreateCategoryRequest);
        Assert.Equal("Hammers", repository.SavedCreateCategoryRequest!.Name);
        Assert.Equal("Hammer family", repository.SavedCreateCategoryRequest.Description);
        Assert.Equal("A1", repository.SavedCreateCategoryRequest.Attributes["aisle"]);
        Assert.Equal(new[] { 2, 7 }, repository.SavedCreateCategoryRequest.ParentCategoryIds);
    }

    [Fact]
    public async Task GetInventoryCountsAsync_NormalizesAttributeAndCategoryFiltersBeforeQuerying()
    {
        var repository = new RecordingInventoryRepository();
        var service = new InventoryManagementService(repository);

        await service.GetInventoryCountsAsync(new InventoryCountRequest
        {
            ProductId = 5,
            Attributes = new Dictionary<string, string>
            {
                [" brand "] = " Acme "
            },
            CategoryIds = new List<int> { 3, 3, 8 },
            MatchAllCategories = true,
            IncludeDescendantCategories = false
        });

        Assert.NotNull(repository.SavedInventoryCountRequest);
        Assert.Equal(5, repository.SavedInventoryCountRequest!.ProductId);
        Assert.Equal("Acme", repository.SavedInventoryCountRequest.Attributes["brand"]);
        Assert.Equal(new[] { 3, 8 }, repository.SavedInventoryCountRequest.CategoryIds);
        Assert.True(repository.SavedInventoryCountRequest.MatchAllCategories);
        Assert.False(repository.SavedInventoryCountRequest.IncludeDescendantCategories);
    }

    private sealed class RecordingInventoryRepository : IInventoryRepository
    {
        public CreateProductRequest? SavedCreateProductRequest { get; private set; }
        public CreateCategoryRequest? SavedCreateCategoryRequest { get; private set; }
        public InventoryAdjustmentRequest? SavedAddInventoryRequest { get; private set; }
        public SearchProductsRequest? SavedSearchProductsRequest { get; private set; }
        public InventoryCountRequest? SavedInventoryCountRequest { get; private set; }

        public Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
        {
            SavedCreateProductRequest = request;
            return Task.FromResult(new ProductResponse
            {
                ProductId = 1,
                Name = request.Name,
                Description = request.Description,
                ProductImageUris = request.ProductImageUris,
                ValidSkus = request.ValidSkus,
                Attributes = request.Attributes,
                CategoryIds = request.CategoryIds
            });
        }

        public Task<ProductResponse> GetProductAsync(int productId)
            => Task.FromResult<ProductResponse>(null!);

        public Task<IReadOnlyList<ProductResponse>> SearchProductsAsync(SearchProductsRequest request)
        {
            SavedSearchProductsRequest = request;
            return Task.FromResult<IReadOnlyList<ProductResponse>>(Array.Empty<ProductResponse>());
        }

        public Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
        {
            SavedCreateCategoryRequest = request;
            return Task.FromResult(new CategoryResponse
            {
                CategoryId = 1,
                Name = request.Name,
                Description = request.Description,
                Attributes = request.Attributes,
                ParentCategoryIds = request.ParentCategoryIds
            });
        }

        public Task<IReadOnlyList<CategoryResponse>> GetCategoriesAsync()
            => Task.FromResult<IReadOnlyList<CategoryResponse>>(Array.Empty<CategoryResponse>());

        public Task<IReadOnlyList<InventoryTransactionResponse>> AddInventoryAsync(InventoryAdjustmentRequest request)
        {
            SavedAddInventoryRequest = request;
            return Task.FromResult<IReadOnlyList<InventoryTransactionResponse>>(Array.Empty<InventoryTransactionResponse>());
        }

        public Task<IReadOnlyList<InventoryTransactionResponse>> RemoveInventoryAsync(InventoryAdjustmentRequest request)
            => Task.FromResult<IReadOnlyList<InventoryTransactionResponse>>(Array.Empty<InventoryTransactionResponse>());

        public Task<bool> DeleteInventoryTransactionAsync(int transactionId)
            => Task.FromResult(true);

        public Task<InventoryCountResponse> GetInventoryCountsAsync(InventoryCountRequest request)
        {
            SavedInventoryCountRequest = request;
            return Task.FromResult(new InventoryCountResponse());
        }
    }
}
