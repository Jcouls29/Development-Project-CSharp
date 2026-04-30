using System;

namespace Sparcpoint.Inventory.Models
{
    // EVAL: Base class mirrors the DB pattern where every entity has an InstanceId
    // and a CreatedTimestamp. This means any new entity (Supplier, Warehouse, etc.)
    // inherits this contract automatically without touching the repository layer.
    public abstract class InstanceBase
    {
        public int InstanceId { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
