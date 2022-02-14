CREATE FUNCTION [Instances].[SearchProductsByCategory]
(
	@categoryName VARCHAR(64)
)
RETURNS @ProductTable TABLE(
	[InstanceId]	   INT,
	[Name]             VARCHAR (256),
    [Description]      VARCHAR (256),
    [ProductImageUris] VARCHAR (MAX),
    [ValidSkus]        VARCHAR (MAX),
    [CreatedTimestamp] DATETIME2 (7)
	)

AS
	BEGIN
	DECLARE @currentCategoryInstanceId INT;

	set @currentCategoryInstanceId = (select InstanceId  from [Instances].Categories where Trim(Name)  = @categoryName);	

	INSERT INTO @ProductTable SELECT InstanceId, Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp 
	from [Instances].[Products] where InstanceId in
	(Select InstanceId from  [Instances].[ProductCategories] where CategoryInstanceId = @currentCategoryInstanceId)

	return
	END
			
Go


