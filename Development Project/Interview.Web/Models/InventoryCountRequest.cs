namespace Interview.Web.Models;

public class InventoryCountRequest
{
    // EVAL: Either ProductInstanceId or AttributeKey/AttributeValue can be used to filter.
    // ProductInstanceId targets a specific product; attribute filters aggregate across
    // all products sharing that metadata (e.g. count all red products).
    public int? ProductInstanceId { get; set; }
    public string AttributeKey { get; set; }
    public string AttributeValue { get; set; }
}
