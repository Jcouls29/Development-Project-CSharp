using Interview.Web.Contracts.Products;
using Interview.Web.Models.Products;
using Interview.Web.Repositories.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<int> AddProductAsync(CreateProductRequest request)
        {
            if (request == null)
            {
                throw new ProductValidationException("Request is required.");
            }

            string name = ValidateRequiredText(request.Name, 256, nameof(request.Name));
            string description = ValidateRequiredText(request.Description, 256, nameof(request.Description));

            var imageUris = ValidateStringCollection(request.ProductImageUris, 2048, nameof(request.ProductImageUris));
            var validSkus = ValidateStringCollection(request.ValidSkus, 128, nameof(request.ValidSkus));

            var metadata = ValidateMetadata(request.Metadata);
            var categoryIds = ValidateCategoryIds(request.CategoryIds);

            var model = new ProductCreateModel
            {
                Name = name,
                Description = description,
                ProductImageUris = imageUris,
                ValidSkus = validSkus,
                Metadata = metadata,
                CategoryIds = categoryIds
            };

            return await _productRepository.AddAsync(model);
        }

        public async Task<IReadOnlyList<SearchProductItemResponse>> SearchProductsAsync(string searchText, string metadataKey, string metadataValue, string categoryIdsCsv)
        {
            string normalizedSearchText = NormalizeOptionalText(searchText, 256, nameof(searchText));
            string normalizedMetadataKey = NormalizeOptionalText(metadataKey, 64, nameof(metadataKey));
            string normalizedMetadataValue = NormalizeOptionalText(metadataValue, 512, nameof(metadataValue));
            var categoryIds = ParseCategoryIds(categoryIdsCsv);

            var request = new ProductSearchRequestModel
            {
                SearchText = normalizedSearchText,
                MetadataKey = normalizedMetadataKey,
                MetadataValue = normalizedMetadataValue,
                CategoryIds = categoryIds
            };

            var products = await _productRepository.SearchAsync(request);
            return products
                .Select(product => new SearchProductItemResponse
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    ProductImageUris = product.ProductImageUris,
                    ValidSkus = product.ValidSkus,
                    CreatedTimestamp = product.CreatedTimestamp,
                    Metadata = product.Metadata
                        .Select(metadata => new SearchProductMetadataResponse
                        {
                            Key = metadata.Key,
                            Value = metadata.Value
                        })
                        .ToList(),
                    CategoryIds = product.CategoryIds
                })
                .ToList();
        }

        private static string ValidateRequiredText(string value, int maxLength, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ProductValidationException($"{fieldName} is required.");
            }

            string trimmed = value.Trim();
            if (trimmed.Length > maxLength)
            {
                throw new ProductValidationException($"{fieldName} must be {maxLength} characters or fewer.");
            }

            return trimmed;
        }

        private static IReadOnlyList<string> ValidateStringCollection(IEnumerable<string> values, int maxLength, string fieldName)
        {
            if (values == null)
            {
                return new List<string>();
            }

            var normalized = new List<string>();
            foreach (string value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                string trimmed = value.Trim();
                if (trimmed.Length > maxLength)
                {
                    throw new ProductValidationException($"{fieldName} contains an item over {maxLength} characters.");
                }

                normalized.Add(trimmed);
            }

            return normalized.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static IReadOnlyList<ProductMetadataModel> ValidateMetadata(IEnumerable<CreateProductAttributeRequest> values)
        {
            if (values == null)
            {
                return new List<ProductMetadataModel>();
            }

            var metadata = new List<ProductMetadataModel>();
            foreach (var item in values)
            {
                if (item == null)
                {
                    continue;
                }

                string key = ValidateRequiredText(item.Key, 64, "Metadata.Key");
                string value = ValidateRequiredText(item.Value, 512, "Metadata.Value");

                if (metadata.Any(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ProductValidationException($"Duplicate metadata key '{key}' is not allowed.");
                }

                metadata.Add(new ProductMetadataModel
                {
                    Key = key,
                    Value = value
                });
            }

            return metadata;
        }

        private static IReadOnlyList<int> ValidateCategoryIds(IEnumerable<int> categoryIds)
        {
            if (categoryIds == null)
            {
                return new List<int>();
            }

            var normalized = categoryIds.Distinct().ToList();
            if (normalized.Any(x => x <= 0))
            {
                throw new ProductValidationException("CategoryIds must be positive integers.");
            }

            return normalized;
        }

        private static string NormalizeOptionalText(string value, int maxLength, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            string trimmed = value.Trim();
            if (trimmed.Length > maxLength)
            {
                throw new ProductValidationException($"{fieldName} must be {maxLength} characters or fewer.");
            }

            return trimmed;
        }

        private static IReadOnlyList<int> ParseCategoryIds(string categoryIdsCsv)
        {
            if (string.IsNullOrWhiteSpace(categoryIdsCsv))
            {
                return new List<int>();
            }

            var ids = new List<int>();
            foreach (string token in categoryIdsCsv.Split(','))
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    continue;
                }

                if (!int.TryParse(token.Trim(), out int parsedId) || parsedId <= 0)
                {
                    throw new ProductValidationException("categoryIds must contain positive integers separated by commas.");
                }

                ids.Add(parsedId);
            }

            return ids.Distinct().ToList();
        }
    }
}
