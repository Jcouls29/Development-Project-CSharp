namespace Sparcpoint.Inventory.Repositories.Sql
{
    /// EVAL: SQL kept in a single const-holder so it is reviewable in one place.
    /// For larger systems, promote to embedded .sql resource files.
    internal static class Queries
    {
        public const string InsertProduct = @"
INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus])
OUTPUT INSERTED.[InstanceId]
VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);";

        public const string InsertProductAttribute = @"
INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
VALUES (@InstanceId, @Key, @Value);";

        public const string InsertProductCategory = @"
INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
VALUES (@InstanceId, @CategoryInstanceId);";

        public const string SelectProductById = @"
SELECT [InstanceId], [Name], [Description], [ProductImageUris], [ValidSkus], [CreatedTimestamp]
FROM [Instances].[Products]
WHERE [InstanceId] = @InstanceId;";

        public const string SelectProductAttributes = @"
SELECT [InstanceId], [Key], [Value]
FROM [Instances].[ProductAttributes]
WHERE [InstanceId] = @InstanceId;";

        public const string SelectProductCategories = @"
SELECT [CategoryInstanceId]
FROM [Instances].[ProductCategories]
WHERE [InstanceId] = @InstanceId;";

        public const string InsertTransaction = @"
INSERT INTO [Transactions].[InventoryTransactions]
    ([ProductInstanceId], [Quantity], [StartedTimestamp], [CompletedTimestamp], [TypeCategory])
OUTPUT INSERTED.[TransactionId]
VALUES (@ProductInstanceId, @Quantity, @StartedTimestamp, @CompletedTimestamp, @TypeCategory);";

        public const string DeleteTransaction = @"
DELETE FROM [Transactions].[InventoryTransactions]
WHERE [TransactionId] = @TransactionId;";

        public const string CountByProduct = @"
SELECT @ProductInstanceId AS [ProductInstanceId],
       ISNULL(SUM([Quantity]), 0) AS [Quantity]
FROM [Transactions].[InventoryTransactions]
WHERE [ProductInstanceId] = @ProductInstanceId
  AND [CompletedTimestamp] IS NOT NULL;";

        public const string InsertCategory = @"
INSERT INTO [Instances].[Categories] ([Name], [Description])
OUTPUT INSERTED.[InstanceId]
VALUES (@Name, @Description);";

        public const string InsertCategoryAttribute = @"
INSERT INTO [Instances].[CategoryAttributes] ([InstanceId], [Key], [Value])
VALUES (@InstanceId, @Key, @Value);";

        public const string InsertCategoryParent = @"
INSERT INTO [Instances].[CategoryCategories] ([InstanceId], [CategoryInstanceId])
VALUES (@InstanceId, @ParentInstanceId);";

        public const string UpdateCategory = @"
UPDATE [Instances].[Categories]
SET [Name] = @Name, [Description] = @Description
WHERE [InstanceId] = @InstanceId;";

        public const string DeleteCategoryAttributes = @"DELETE FROM [Instances].[CategoryAttributes] WHERE [InstanceId] = @InstanceId;";
        public const string DeleteCategoryParents = @"DELETE FROM [Instances].[CategoryCategories] WHERE [InstanceId] = @InstanceId;";
        public const string DeleteCategory = @"DELETE FROM [Instances].[Categories] WHERE [InstanceId] = @InstanceId;";

        public const string SelectCategoryById = @"
SELECT [InstanceId], [Name], [Description], [CreatedTimestamp]
FROM [Instances].[Categories]
WHERE [InstanceId] = @InstanceId;";

        public const string SelectCategoryAttributes = @"
SELECT [Key], [Value]
FROM [Instances].[CategoryAttributes]
WHERE [InstanceId] = @InstanceId;";

        public const string SelectCategoryParents = @"
SELECT [CategoryInstanceId]
FROM [Instances].[CategoryCategories]
WHERE [InstanceId] = @InstanceId;";

        public const string SelectAllCategories = @"
SELECT [InstanceId], [Name], [Description], [CreatedTimestamp]
FROM [Instances].[Categories]
ORDER BY [InstanceId];";

        // EVAL: Recursive CTE walks child -> descendants via CategoryCategories.
        // InstanceId column is the child; CategoryInstanceId is the parent.
        public const string SelectCategoryDescendants = @"
WITH Descendants AS (
    SELECT cc.[InstanceId]
    FROM [Instances].[CategoryCategories] cc
    WHERE cc.[CategoryInstanceId] = @RootId
    UNION ALL
    SELECT cc.[InstanceId]
    FROM [Instances].[CategoryCategories] cc
    INNER JOIN Descendants d ON cc.[CategoryInstanceId] = d.[InstanceId]
)
SELECT c.[InstanceId], c.[Name], c.[Description], c.[CreatedTimestamp]
FROM [Instances].[Categories] c
INNER JOIN Descendants d ON d.[InstanceId] = c.[InstanceId];";

        public const string CountByAttribute = @"
SELECT p.[InstanceId] AS [ProductInstanceId],
       ISNULL(SUM(t.[Quantity]), 0) AS [Quantity]
FROM [Instances].[Products] p
INNER JOIN [Instances].[ProductAttributes] a
    ON a.[InstanceId] = p.[InstanceId]
LEFT JOIN [Transactions].[InventoryTransactions] t
    ON t.[ProductInstanceId] = p.[InstanceId]
    AND t.[CompletedTimestamp] IS NOT NULL
WHERE a.[Key] = @Key AND a.[Value] = @Value
GROUP BY p.[InstanceId];";
    }
}
