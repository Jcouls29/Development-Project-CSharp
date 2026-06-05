namespace Interview.Web.Models;

public class ProductSearchRequest
{
    public string Name { get; set; }
    public string Description { get; set; }

    // EVAL: Dictionary<string, string> doesn't bind from [FromQuery] so flat strings are used
    // instead. To support multiple attribute filters, these could become string[] AttributeKeys
    // and string[] AttributeValues using repeated params (?attributeKeys=color&attributeKeys=brand)
    // without breaking existing callers.
    public string AttributeKey { get; set; }
    public string AttributeValue { get; set; }

    public int[] CategoryIds { get; set; }
}
