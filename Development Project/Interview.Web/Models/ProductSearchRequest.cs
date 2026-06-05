namespace Interview.Web.Models;

public class ProductSearchRequest
{
    public string Name { get; set; }
    public string Description { get; set; }

    // EVAL: AttributeKey and AttributeValue are used instead of Dictionary<string, string>
    // because Dictionary does not bind correctly from query string parameters with [FromQuery].
    // A single key/value pair covers the primary use case and keeps the API simple to call.
    public string AttributeKey { get; set; }
    public string AttributeValue { get; set; }

    public int[] CategoryIds { get; set; }
}
