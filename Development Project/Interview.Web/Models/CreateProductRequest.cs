using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models;

public sealed class CreateProductRequest
{
    [Required]
    [MaxLength(256)]
    public required string Name { get; init; }

    [Required]
    [MaxLength(256)]
    public required string Description { get; init; }

    [Required]
    public required List<string> ProductImageUris { get; init; }

    [Required]
    public required List<string> ValidSkus { get; init; }

    [Required]
    public required List<CreateProductAttributeRequest> Attributes { get; init; }

    [Required]
    public required List<int> CategoryIds { get; init; }
}

public sealed class CreateProductAttributeRequest
{
    [Required]
    [MaxLength(64)]
    public required string Key { get; init; }

    [Required]
    [MaxLength(512)]
    public required string Value { get; init; }
}
