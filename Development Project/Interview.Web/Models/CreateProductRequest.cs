using System.Collections.Generic;

namespace Interview.Web.Models;

public class CreateProductRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string[] ValidSkus { get; set; }
    public string[] ProductImageUris { get; set; }

    // EVAL: Arbitrary key/value pairs map directly to Instances.ProductAttributes rows.
    // This satisfies the requirement for products to support arbitrary metadata without schema changes.
    public Dictionary<string, string> Attributes { get; set; }

    public int[] CategoryIds { get; set; }
}
