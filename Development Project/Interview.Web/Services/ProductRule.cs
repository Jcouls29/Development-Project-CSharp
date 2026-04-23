using Interview.Web.Models.Product;
using System;

namespace Interview.Web.Services
{
    public class ProductRule : IProductRule
    {
        public void ValidateCreate(CreateProductRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Product name is required", nameof(request.Name));

            // Optional: length validation (safe guard)
            if (request.Name.Length > 200)
                throw new ArgumentException("Product name is too long", nameof(request.Name));

            if (request.Description != null && request.Description.Length > 1000)
                throw new ArgumentException("Description is too long", nameof(request.Description));

            // EVAL: Prevent empty metadata keys
            if (request.Metadata != null)
            {
                foreach (var attr in request.Metadata)
                {
                    if (string.IsNullOrWhiteSpace(attr.Key))
                        throw new ArgumentException("Metadata key cannot be empty");

                    if (string.IsNullOrWhiteSpace(attr.Value))
                        throw new ArgumentException("Metadata value cannot be empty");
                }
            }
        }

        public void ValidateSearch(ProductSearchRequest request)
        {
            if (request == null)
                return; // search can be empty → return all

            if (request.CategoryId.HasValue && request.CategoryId <= 0)
                throw new ArgumentException("Invalid CategoryId", nameof(request.CategoryId));

            // Optional: prevent useless query
            if (string.IsNullOrWhiteSpace(request.Name) &&
                !request.CategoryId.HasValue &&
                string.IsNullOrWhiteSpace(request.AttributeKey) &&
                string.IsNullOrWhiteSpace(request.AttributeValue))
            {
                // EVAL: Allowing empty search to return all products (can be changed based on requirement)
            }
        }
    }
}
