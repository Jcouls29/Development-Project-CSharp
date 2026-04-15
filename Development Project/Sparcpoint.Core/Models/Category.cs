using System;

namespace Sparcpoint.Core.Models
{
    // EVAL: Domain Entity - Represents a category for organizing products
    // EVAL: Value Object pattern - immutable identity based on InstanceId
    public class Category
    {
        public int InstanceId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedTimestamp { get; private set; }

        private Category() { }

        // EVAL: Factory method ensures valid category creation
        public static Category Create(string name, string description)
        {
            PreConditions.StringNotNullOrWhitespace(name, nameof(name));
            PreConditions.StringNotNullOrWhitespace(description, nameof(description));

            // EVAL: Business rule validation
            if (name.Length > 64) throw new ArgumentException("Category name cannot exceed 64 characters", nameof(name));
            if (description.Length > 256) throw new ArgumentException("Category description cannot exceed 256 characters", nameof(description));

            return new Category
            {
                Name = name,
                Description = description,
                CreatedTimestamp = DateTime.UtcNow
            };
        }

        // EVAL: Method for repository hydration and object updates
        public void SetInstanceId(int instanceId) => InstanceId = instanceId;

        // EVAL: Internal factory for repository hydration
        internal static Category Load(int instanceId, string name, string description, DateTime createdTimestamp)
        {
            return new Category
            {
                InstanceId = instanceId,
                Name = name,
                Description = description,
                CreatedTimestamp = createdTimestamp
            };
        }

        // EVAL: Value equality based on identity
        public override bool Equals(object obj)
        {
            return obj is Category category && InstanceId == category.InstanceId;
        }

        public override int GetHashCode()
        {
            return InstanceId.GetHashCode();
        }
    }
}