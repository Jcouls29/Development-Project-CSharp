
CREATE PROCEDURE [dbo].[usp_AddProduct]
    @Name int,
	@Description varchar(256),
	@ProductImageUris varchar(max),
	@ValidSkus varchar(max),
	@CreatedTimestamp datetime2(7)

    AS

    SET NOCOUNT ON
    INSERT INTO [Instances].[Products]
           ([Name]
           ,[Description]
           ,[ProductImageUris]
           ,[ValidSkus]
           ,[CreatedTimestamp])
     VALUES
           (@Name,
		   @Description,
		   @ProductImageUris,
		   @ValidSkus,
		   @CreatedTimestamp)

    SELECT @@IDENTITY AS 'AddedProductId';

GO


