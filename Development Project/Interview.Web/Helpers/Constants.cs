
namespace Interview.Web.Helpers
{
    /// <summary>
    /// Class to hold hard coded string
    /// </summary>
    public static class Constants
    {
        #region Hardcoded strings
        public const string MessageIsAvailable = "Readiness - Controller is available";
        public const string DefaultConnection = "DefaultConnection";
        #endregion

        #region SQL

        // Product
        public const string ProductGetAllActive = "Select * from Instances.Products Where ValidSkus != 'DELETED'";

        public const string ProductInsert = "Insert into " +
                "Instances.Products (Name, Description, ProductImageUris, ValidSkus) " +
                "Values(@Name, @Description, @ProductImageUris, @ValidSkus);" +
                "select cast(scope_identity() as int);";

        public const string ProductFlagAsDeleted = "Update Instances.Products Set ValidSkus = 'DELETED' Where InstanceId=@_itemId";

        public const string ProductCountAllActive = "Select count(*) from Instances.Products Where ValidSkus != 'DELETED'";

        public const string ProductGetInventory = "select p.* " +
            "FROM Instances.Products p " +
            "inner join Instances.ProductCategories pc on p.InstanceId = pc.InstanceId " +
            "inner join Instances.Categories c on c.InstanceId = pc.CategoryInstanceId " +
            "WHERE p.Name = ISNULL(@Name, p.Name) " +
            "AND p.Description = ISNULL(@Description, p.Description) " +
            "AND p.ValidSkus = ISNULL(@ValidSkus, p.ValidSkus) " +
            "AND c.Name = ISNULL(@CategoryName, c.Name)";

        // Categories
        public const string CategoryInsert = "Insert into " +
                "Instances.Categories (Name, Description) Values(@Name, @Description);" +
                "select cast(scope_identity() as int);";

        public const string CategoryInsertIfNotExist = "if Not Exists (select * from Instances.Categories where Name = @Name) " +
            "begin insert into Instances.Categories(Name, Description) values(@Name, @Description); " +
            "end else begin " + "select * from Instances.Categories where Name = @Name; end" +
            " select cast(scope_identity() as int);";

        public const string CategoryGetAll = "Select * from Instances.Categories";

        // ProductCategories
        public const string ProductCategoriesInsert = "Insert into " +
                "Instances.ProductCategories (InstanceId, CategoryInstanceId) Values(@InstanceId, @CategoryInstanceId)";

        #endregion

    }
}
