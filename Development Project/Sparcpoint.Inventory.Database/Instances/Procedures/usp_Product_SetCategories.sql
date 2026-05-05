CREATE PROCEDURE [Instances].[usp_Product_SetCategories]
    @ProductInstanceId INT,
    @CategoryIds dbo.IntegerList READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM [Instances].[Products] WHERE [InstanceId] = @ProductInstanceId)
            THROW 51000, 'Product not found', 1;

        DELETE FROM [Instances].[ProductCategories]
        WHERE [InstanceId] = @ProductInstanceId;

        INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
        SELECT DISTINCT @ProductInstanceId, cid.[Value]
        FROM @CategoryIds cid
        INNER JOIN [Instances].[Categories] c ON c.[InstanceId] = cid.[Value];

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO