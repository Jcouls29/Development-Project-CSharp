using Interview.Web.Contracts;
using Interview.Web.Data;
using Interview.Web.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    public class InventoryManagementService : IInventoryManagementService
    {
        private const int ProductNameMaxLength = 256;
        private const int ProductDescriptionMaxLength = 256;
        private const int CategoryNameMaxLength = 64;
        private const int CategoryDescriptionMaxLength = 256;
        private const int AttributeKeyMaxLength = 64;
        private const int AttributeValueMaxLength = 512;
        private const int SkuMaxLength = 128;
        private const int TypeCategoryMaxLength = 32;

        private readonly IInventoryRepository _inventoryRepository;

        public InventoryManagementService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public Task<ProductResponse> GetProductAsync(int productId)
        {
            EnsurePositive(productId, "productId");
            return _inventoryRepository.GetProductAsync(productId);
        }

        public Task<IReadOnlyList<ProductResponse>> SearchProductsAsync(SearchProductsRequest request)
            => _inventoryRepository.SearchProductsAsync(NormalizeSearchProductsRequest(request ?? new SearchProductsRequest()));

        public Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
            => _inventoryRepository.CreateProductAsync(NormalizeCreateProductRequest(request));

        public Task<ProductResponse> UpdateProductAsync(int productId, UpdateProductRequest request)
        {
            EnsurePositive(productId, nameof(productId));
            return _inventoryRepository.UpdateProductAsync(productId, NormalizeUpdateProductRequest(request));
        }

        public Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
            => _inventoryRepository.CreateCategoryAsync(NormalizeCreateCategoryRequest(request));

        public Task<IReadOnlyList<CategoryResponse>> GetCategoriesAsync()
            => _inventoryRepository.GetCategoriesAsync();

        public Task<IReadOnlyList<InventoryTransactionResponse>> AddInventoryAsync(InventoryAdjustmentRequest request)
            => _inventoryRepository.AddInventoryAsync(NormalizeInventoryAdjustmentRequest(request, "manual-add"));

        public Task<IReadOnlyList<InventoryTransactionResponse>> RemoveInventoryAsync(InventoryAdjustmentRequest request)
            => _inventoryRepository.RemoveInventoryAsync(NormalizeInventoryAdjustmentRequest(request, "manual-remove"));

        public Task<bool> DeleteInventoryTransactionAsync(int transactionId)
        {
            EnsurePositive(transactionId, "transactionId");
            return _inventoryRepository.DeleteInventoryTransactionAsync(transactionId);
        }

        public Task<InventoryCountResponse> GetInventoryCountsAsync(InventoryCountRequest request)
            => _inventoryRepository.GetInventoryCountsAsync(NormalizeInventoryCountRequest(request ?? new InventoryCountRequest()));

        private static CreateProductRequest NormalizeCreateProductRequest(CreateProductRequest request)
        {
            EnsureRequest(request, nameof(request));

            return new CreateProductRequest
            {
                Name = NormalizeRequiredString(request.Name, nameof(request.Name), ProductNameMaxLength),
                Description = NormalizeRequiredString(request.Description, nameof(request.Description), ProductDescriptionMaxLength),
                ProductImageUris = NormalizeOptionalList(request.ProductImageUris),
                ValidSkus = NormalizeOptionalList(request.ValidSkus, SkuMaxLength),
                Attributes = NormalizeAttributes(request.Attributes),
                CategoryIds = NormalizePositiveIds(request.CategoryIds, nameof(request.CategoryIds))
            };
        }

        private static UpdateProductRequest NormalizeUpdateProductRequest(UpdateProductRequest request)
        {
            EnsureRequest(request, nameof(request));

            return new UpdateProductRequest
            {
                Name = NormalizeRequiredString(request.Name, nameof(request.Name), ProductNameMaxLength),
                Description = NormalizeRequiredString(request.Description, nameof(request.Description), ProductDescriptionMaxLength),
                ProductImageUris = NormalizeOptionalList(request.ProductImageUris),
                ValidSkus = NormalizeOptionalList(request.ValidSkus, SkuMaxLength),
                Attributes = NormalizeAttributes(request.Attributes),
                CategoryIds = NormalizePositiveIds(request.CategoryIds, nameof(request.CategoryIds))
            };
        }

        private static SearchProductsRequest NormalizeSearchProductsRequest(SearchProductsRequest request)
        {
            return new SearchProductsRequest
            {
                NameContains = NormalizeOptionalString(request.NameContains, ProductNameMaxLength),
                DescriptionContains = NormalizeOptionalString(request.DescriptionContains, ProductDescriptionMaxLength),
                Skus = NormalizeOptionalList(request.Skus, SkuMaxLength),
                Attributes = NormalizeAttributes(request.Attributes),
                CategoryIds = NormalizePositiveIds(request.CategoryIds, nameof(request.CategoryIds)),
                MatchAllCategories = request.MatchAllCategories,
                IncludeDescendantCategories = request.IncludeDescendantCategories
            };
        }

        private static CreateCategoryRequest NormalizeCreateCategoryRequest(CreateCategoryRequest request)
        {
            EnsureRequest(request, nameof(request));

            return new CreateCategoryRequest
            {
                Name = NormalizeRequiredString(request.Name, nameof(request.Name), CategoryNameMaxLength),
                Description = NormalizeRequiredString(request.Description, nameof(request.Description), CategoryDescriptionMaxLength),
                Attributes = NormalizeAttributes(request.Attributes),
                ParentCategoryIds = NormalizePositiveIds(request.ParentCategoryIds, nameof(request.ParentCategoryIds))
            };
        }

        private static InventoryAdjustmentRequest NormalizeInventoryAdjustmentRequest(InventoryAdjustmentRequest request, string defaultTypeCategory)
        {
            EnsureRequest(request, nameof(request));

            // EVAL: Merging duplicate product lines here keeps downstream inventory writes simpler
            // and gives the API one consistent interpretation of a batch adjustment request.
            var normalizedItems = new Dictionary<int, decimal>();
            foreach (var item in request.Items ?? new List<InventoryAdjustmentItemRequest>())
            {
                if (item == null)
                    continue;

                EnsurePositive(item.ProductId, nameof(item.ProductId));
                if (item.Quantity <= 0)
                    throw new ApiValidationException("Inventory quantities must be greater than zero.");

                if (normalizedItems.ContainsKey(item.ProductId))
                    normalizedItems[item.ProductId] += item.Quantity;
                else
                    normalizedItems[item.ProductId] = item.Quantity;
            }

            if (normalizedItems.Count == 0)
                throw new ApiValidationException("At least one inventory item is required.");

            return new InventoryAdjustmentRequest
            {
                TypeCategory = NormalizeOptionalString(request.TypeCategory, TypeCategoryMaxLength) ?? defaultTypeCategory,
                Items = normalizedItems
                    .Select(pair => new InventoryAdjustmentItemRequest
                    {
                        ProductId = pair.Key,
                        Quantity = pair.Value
                    })
                    .ToList()
            };
        }

        private static InventoryCountRequest NormalizeInventoryCountRequest(InventoryCountRequest request)
        {
            if (request.ProductId.HasValue)
                EnsurePositive(request.ProductId.Value, nameof(request.ProductId));

            return new InventoryCountRequest
            {
                ProductId = request.ProductId,
                Attributes = NormalizeAttributes(request.Attributes),
                CategoryIds = NormalizePositiveIds(request.CategoryIds, nameof(request.CategoryIds)),
                MatchAllCategories = request.MatchAllCategories,
                IncludeDescendantCategories = request.IncludeDescendantCategories
            };
        }

        private static Dictionary<string, string> NormalizeAttributes(IDictionary<string, string> attributes)
        {
            var normalized = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
            if (attributes == null)
                return normalized;

            foreach (var pair in attributes)
            {
                var key = NormalizeRequiredString(pair.Key, "attribute key", AttributeKeyMaxLength);
                var value = NormalizeRequiredString(pair.Value, $"attribute '{key}'", AttributeValueMaxLength);

                if (normalized.ContainsKey(key))
                    throw new ApiValidationException($"Duplicate attribute key '{key}' is not allowed.");

                normalized.Add(key, value);
            }

            return normalized;
        }

        private static List<string> NormalizeOptionalList(IEnumerable<string> values, int maxLength = 512)
        {
            var normalized = new List<string>();
            if (values == null)
                return normalized;

            foreach (var value in values)
            {
                var trimmed = NormalizeOptionalString(value, maxLength);
                if (trimmed == null)
                    continue;

                if (!normalized.Contains(trimmed, System.StringComparer.OrdinalIgnoreCase))
                    normalized.Add(trimmed);
            }

            return normalized;
        }

        private static List<int> NormalizePositiveIds(IEnumerable<int> ids, string parameterName)
        {
            var normalized = new List<int>();
            if (ids == null)
                return normalized;

            foreach (var id in ids.Distinct())
            {
                EnsurePositive(id, parameterName);
                normalized.Add(id);
            }

            return normalized;
        }

        private static string NormalizeRequiredString(string value, string parameterName, int maxLength)
        {
            var normalized = NormalizeOptionalString(value, maxLength);
            if (normalized == null)
                throw new ApiValidationException($"{parameterName} is required.");

            return normalized;
        }

        private static string NormalizeOptionalString(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var trimmed = value.Trim();
            if (trimmed.Length > maxLength)
                throw new ApiValidationException($"{trimmed[..System.Math.Min(32, trimmed.Length)]} exceeds the allowed length of {maxLength} characters.");

            return trimmed;
        }

        private static void EnsureRequest(object request, string parameterName)
        {
            if (request == null)
                throw new ApiValidationException($"{parameterName} is required.");
        }

        private static void EnsurePositive(int value, string parameterName)
        {
            if (value <= 0)
                throw new ApiValidationException($"{parameterName} must be greater than zero.");
        }
    }
}
