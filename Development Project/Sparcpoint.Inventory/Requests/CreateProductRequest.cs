using System.Collections.Generic;

namespace Sparcpoint.Inventory.Requests
{
    // EVAL: Separate request model from the domain model so API shape
    // can evolve independently of the DB entity without breaking either side.
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> ProductImageUris { get; set; } = new();
        public List<string> ValidSkus { get; set; } = new();

        // EVAL: Arbitrary key/value metadata — maps directly to ProductAttributes table.
        public Dictionary<string, string> Attributes { get; set; } = new();

        // EVAL: Categories are assigned by existing InstanceId.
        // Creating new categories in the same request is out of scope per the spec.
        public List<int> CategoryInstanceIds { get; set; } = new();
    }
}
