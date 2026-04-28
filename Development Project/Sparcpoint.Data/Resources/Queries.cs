namespace Sparcpoint.Data.Resources
{
    internal static class Queries
    {
        internal const string GetAllProducts = "SELECT InstanceId, [Name], [Description], ProductImageUris, ValidSkus, CreatedTimestamp FROM Instances.Products;";
        internal const string GetProductById = "SELECT InstanceId, [Name], [Description], ProductImageUris, ValidSkus, CreatedTimestamp FROM Instances.Products WHERE InstanceId = @id;";
        internal const string AddProduct = "INSERT INTO Instances.Products ([Name], [Description], ProductImageUris, ValidSkus) VALUES (@name, @description, @productImageUris, @validskus); SELECT CAST(SCOPE_IDENTITY() as int);";
        internal const string GetProductCount = "SELECT COUNT(InstanceId) FROM Instances.Products;";
    }
}
