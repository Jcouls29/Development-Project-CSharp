namespace Interview.Application.UseCases.Exception;

public sealed class DuplicateProductNameException : System.Exception
{
    public DuplicateProductNameException()
        : base("A product with the same name already exists.")
    {
    }
}
