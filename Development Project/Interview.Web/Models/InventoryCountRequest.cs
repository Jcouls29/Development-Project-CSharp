namespace Interview.Web.Models;

public class InventoryCountRequest
{
    // EVAL: Filter by product ID for a specific count, or by attribute key/value to
    // aggregate across all matching products (e.g. total stock of all red products).
    public int? ProductInstanceId { get; set; }
    public string AttributeKey { get; set; }
    public string AttributeValue { get; set; }
}
