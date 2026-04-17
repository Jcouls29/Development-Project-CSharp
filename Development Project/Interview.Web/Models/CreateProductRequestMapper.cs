using Interview.Application.UseCases.Command;

namespace Interview.Web.Models;

public static class CreateProductRequestMapper
{
    public static CreateProductCommand ToCommand(this CreateProductRequest request)
    {
        var imageUris = request.ProductImageUris ?? new List<string>();
        var skus = request.ValidSkus ?? new List<string>();
        var attributes = (request.Attributes ?? new List<CreateProductAttributeRequest>())
            .Select(a => new CreateProductAttributeItem { Key = a.Key, Value = a.Value })
            .ToList();
        var categoryIds = request.CategoryIds ?? new List<int>();

        return new CreateProductCommand
        {
            Name = request.Name,
            Description = request.Description,
            ProductImageUris = imageUris,
            ValidSkus = skus,
            Attributes = attributes,
            CategoryIds = categoryIds,
        };
    }
}
