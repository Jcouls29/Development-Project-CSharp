namespace Sparcpoint.Inventory.Models
{
    [Flags]
    public enum ProductSearchScope
    {
        Attribute,
        Category,
        Description,
        Name,
        ValidSkus,
        Any = Attribute | Category | Description | Name | ValidSkus
    }
}