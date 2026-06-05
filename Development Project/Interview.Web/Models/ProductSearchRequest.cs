namespace Interview.Web.Models;

public class ProductSearchRequest
{
    public string Name { get; set; }
    public string Description { get; set; }

    // EVAL: AttributeKey and AttributeValue are used instead of Dictionary<string, string>
    // because Dictionary does not bind correctly from query string parameters with [FromQuery].
    // A single key/value pair satisfies the spec requirement of searchable by metadata.
    // To support multiple attribute filters in the future, these could be changed to
    // string[] AttributeKeys and string[] AttributeValues using repeated query parameters
    // (e.g. ?attributeKeys=color&attributeKeys=brand&attributeValues=red&attributeValues=Nike)
    // without breaking existing callers using a single pair.
    public string AttributeKey { get; set; }
    public string AttributeValue { get; set; }

    public int[] CategoryIds { get; set; }
}
