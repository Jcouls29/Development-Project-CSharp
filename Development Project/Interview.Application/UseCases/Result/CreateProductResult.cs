namespace Interview.Application.UseCases.Result;

public abstract record CreateProductResult
{
    private CreateProductResult() { }

    public sealed record Succeeded(int ProductInstanceId) : CreateProductResult;

    public sealed record InvalidName : CreateProductResult;

    public sealed record DuplicateName : CreateProductResult;

    public sealed record DuplicateAttributeKeys(List<string> Keys) : CreateProductResult;

    public sealed record InvalidCategories(List<int> MissingCategoryIds) : CreateProductResult;
}
