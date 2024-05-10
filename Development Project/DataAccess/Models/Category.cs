using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models;

public class Category
{
    public int Id { get; init; }
    [MaxLength(64)]
    public string Name { get; init; } = string.Empty;
    [MaxLength(256)]
    public string Description { get; init; } = string.Empty;
    public DateTime CreatedTimestamp { get; init; }
    
    public int? ParentCategoryId { get; init; }
    public Category? ParentCategory { get; init; }
}