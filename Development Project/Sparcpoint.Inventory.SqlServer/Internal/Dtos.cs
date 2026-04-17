using System;

namespace Sparcpoint.Inventory.SqlServer.Internal
{
    /// <summary>
    /// EVAL: Internal DTOs that map row-by-row with SELECT. Isolated from
    /// the public domain so the schema can change without breaking contracts.
    /// </summary>
    internal sealed class ProductRow
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }

    internal sealed class ProductAttributeRow
    {
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    internal sealed class ProductCategoryRow
    {
        public int InstanceId { get; set; }
        public int CategoryInstanceId { get; set; }
    }

    internal sealed class CategoryRow
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }

    internal sealed class CategoryAttributeRow
    {
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    internal sealed class CategoryCategoryRow
    {
        public int InstanceId { get; set; }
        public int CategoryInstanceId { get; set; }
    }
}
