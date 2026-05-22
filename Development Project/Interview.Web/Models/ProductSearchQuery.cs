using System.Collections.Generic;
public class ProductSearchQuery
{
    public string? Query { get; set; }
    public string? Category { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}