namespace DataAccess.Models;

public class Metadata
{
    public int Id { get; init; }
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    
    public int ProductId { get; set; }
    public Product Product { get; set; }
}