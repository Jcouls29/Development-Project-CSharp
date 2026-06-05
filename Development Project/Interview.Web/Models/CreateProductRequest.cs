using System.Collections.Generic;

namespace Interview.Web.Models;

public class CreateProductRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string[] ValidSkus { get; set; }
    public string[] ProductImageUris { get; set; }

    // EVAL: Key/value pairs map to Instances.ProductAttributes rows — one row per entry.
    public Dictionary<string, string> Attributes { get; set; }

    public int[] CategoryIds { get; set; }
}
