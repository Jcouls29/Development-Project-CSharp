using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Repositories;

namespace Sparcpoint.Inventory.Services
{
    /// <summary>
    /// EVAL: Implements product business logic with input validation.
    /// Uses PreConditions from Sparcpoint.Core for parameter validation.
    /// Validates string lengths against database column limits to prevent SQL truncation errors.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _ProductRepository;

        private const int MaxNameLength = 256;
        private const int MaxDescriptionLength = 256;

        public ProductService(IProductRepository productRepository)
        {
            _ProductRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            PreConditions.ParameterNotNull(product, nameof(product));
            PreConditions.StringNotNullOrWhitespace(product.Name, nameof(product.Name));
            PreConditions.StringNotNullOrWhitespace(product.Description, nameof(product.Description));

            // EVAL: Validate string lengths against database column constraints to prevent silent truncation
            if (product.Name.Length > MaxNameLength)
                throw new ArgumentException($"Product name cannot exceed {MaxNameLength} characters.", nameof(product.Name));

            if (product.Description.Length > MaxDescriptionLength)
                throw new ArgumentException($"Product description cannot exceed {MaxDescriptionLength} characters.", nameof(product.Description));

            // EVAL: Validate attribute key/value lengths match [ProductAttributes] column constraints
            if (product.Attributes != null)
            {
                foreach (var attr in product.Attributes)
                {
                    if (string.IsNullOrWhiteSpace(attr.Key))
                        throw new ArgumentException("Attribute key cannot be empty.");
                    if (attr.Key.Length > 64)
                        throw new ArgumentException($"Attribute key '{attr.Key}' exceeds maximum length of 64 characters.");
                    if (attr.Value?.Length > 512)
                        throw new ArgumentException($"Attribute value for key '{attr.Key}' exceeds maximum length of 512 characters.");
                }
            }

            return await _ProductRepository.CreateAsync(product);
        }

        public async Task<Product?> GetProductByIdAsync(int instanceId)
        {
            if (instanceId <= 0)
                throw new ArgumentException("Instance ID must be a positive integer.", nameof(instanceId));

            return await _ProductRepository.GetByIdAsync(instanceId);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(ProductSearchCriteria criteria)
        {
            PreConditions.ParameterNotNull(criteria, nameof(criteria));
            return await _ProductRepository.SearchAsync(criteria);
        }
    }
}
