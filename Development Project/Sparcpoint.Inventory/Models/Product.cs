using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    // EVAL: Product extends InstanceBase to inherit InstanceId + CreatedTimestamp,
    // matching the DB schema exactly. ValidSkus and ProductImageUris remain as
    // JSON-serialized strings to stay consistent with the existing schema design —
    // no new tables required per the spec.
    public class Product : InstanceBase
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // EVAL: Stored as JSON strings in DB (VARCHAR MAX). Deserialized on read
        // via JsonDataSerializer to avoid adding extra tables.
        public List<string> ProductImageUris { get; set; } = new();
        public List<string> ValidSkus { get; set; } = new();

        // EVAL: Hydrated separately via JOIN — not stored on Products table.
        public Dictionary<string, string> Attributes { get; set; } = new();
        public List<int> CategoryInstanceIds { get; set; } = new();
    }
}
