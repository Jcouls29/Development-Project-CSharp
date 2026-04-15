using System;
using System.Collections.Generic;

namespace Sparcpoint.Core.Models
{
    // EVAL: Domain Entity - Represents a product in the inventory system
    // EVAL: Uses encapsulation to protect business rules and invariants
    public class Product
    {
        public int InstanceId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<string> ProductImageUris { get; private set; } = new List<string>();
        public List<string> ValidSkus { get; private set; } = new List<string>();
        public DateTime CreatedTimestamp { get; private set; }
        public List<Category> Categories { get; private set; } = new List<Category>();
        public Dictionary<string, string> Metadata { get; private set; } = new Dictionary<string, string>();

        // EVAL: Private constructor enforces factory method pattern for controlled instantiation
        private Product() { }

        // EVAL: Factory method ensures valid object creation with business rules
        public static Product Create(string name, string description, IEnumerable<string> productImageUris = null, IEnumerable<string> validSkus = null)
        {
            PreConditions.StringNotNullOrWhitespace(name, nameof(name));
            PreConditions.StringNotNullOrWhitespace(description, nameof(description));

            // EVAL: Business rule validation - enforce reasonable string lengths
            if (name.Length > 256) throw new ArgumentException("Product name cannot exceed 256 characters", nameof(name));
            if (description.Length > 256) throw new ArgumentException("Product description cannot exceed 256 characters", nameof(description));

            return new Product
            {
                Name = name,
                Description = description,
                ProductImageUris = productImageUris != null ? new List<string>(productImageUris) : new List<string>(),
                ValidSkus = validSkus != null ? new List<string>(validSkus) : new List<string>(),
                CreatedTimestamp = DateTime.UtcNow
            };
        }

        // EVAL: Internal factory for repository hydration using fully loaded data
        internal static Product Load(int instanceId, string name, string description, IEnumerable<string> productImageUris,
            IEnumerable<string> validSkus, DateTime createdTimestamp, IEnumerable<Category> categories,
            Dictionary<string, string> metadata)
        {
            var product = new Product
            {
                Name = name,
                Description = description,
                ProductImageUris = productImageUris != null ? new List<string>(productImageUris) : new List<string>(),
                ValidSkus = validSkus != null ? new List<string>(validSkus) : new List<string>(),
                CreatedTimestamp = createdTimestamp,
                Metadata = metadata != null ? new Dictionary<string, string>(metadata) : new Dictionary<string, string>()
            };

            product.SetInstanceId(instanceId);

            if (categories != null)
            {
                foreach (var category in categories)
                {
                    if (!product.Categories.Contains(category))
                        product.Categories.Add(category);
                }
            }

            return product;
        }

        // EVAL: Public method to allow setting identity during update or hydration while protecting immutability after set
        public void SetInstanceId(int instanceId)
        {
            if (InstanceId != 0 && InstanceId != instanceId)
                throw new InvalidOperationException("InstanceId is already set");

            InstanceId = instanceId;
        }

        // EVAL: Business methods for category management
        public void AddCategory(Category category)
        {
            PreConditions.ParameterNotNull(category, nameof(category));
            if (!Categories.Contains(category))
                Categories.Add(category);
        }

        public void RemoveCategory(Category category)
        {
            Categories.Remove(category);
        }

        // EVAL: Business methods for metadata management (key-value pairs)
        public void SetMetadata(string key, string value)
        {
            PreConditions.StringNotNullOrWhitespace(key, nameof(key));
            PreConditions.ParameterNotNull(value, nameof(value)); // Allow empty string but not null

            // EVAL: Business rule - reasonable key/value length limits
            if (key.Length > 64) throw new ArgumentException("Metadata key cannot exceed 64 characters", nameof(key));
            if (value.Length > 512) throw new ArgumentException("Metadata value cannot exceed 512 characters", nameof(value));

            Metadata[key] = value;
        }

        public void RemoveMetadata(string key)
        {
            Metadata.Remove(key);
        }
    }
}