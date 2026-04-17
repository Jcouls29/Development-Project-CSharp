using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    /// <summary>
    /// EVAL: Represents a domain product. Products are never deleted (requirement #1),
    /// only added. Arbitrary metadata lives in <see cref="Attributes"/> and the relation
    /// to categories in <see cref="CategoryIds"/>.
    /// Mirrors [Instances].[Products].
    /// </summary>
    public sealed class Product
    {
        /// <summary>Primary key assigned by the DB (IDENTITY). 0 for a non-persisted product.</summary>
        public int InstanceId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// EVAL: Stored as JSON/VARCHAR(MAX) in the DB; here we expose it as a list
        /// to make consumption easier from controllers and tests.
        /// </summary>
        public IList<string> ProductImageUris { get; set; } = new List<string>();

        public IList<string> ValidSkus { get; set; } = new List<string>();

        public DateTime CreatedTimestamp { get; set; }

        public IList<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();

        public IList<int> CategoryIds { get; set; } = new List<int>();
    }
}
