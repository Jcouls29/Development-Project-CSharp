----------------------------------------------------------
--Created By: Jeremy Hill 20230729
--Purpose: To Add New Products to the system
--Note:  Formatted using Poor Man's T-SQL formatter
--Example: 
--AddProduct '{"categoryname": "Category XYZ", "name": "product1", "description": "This is the product1", "productimageuris": "http://test.com", "validskus" : "100,1011,2222", "attributes": [{"key": "size","value": "100mm"},{"key": "Manufacturer","value": "Acme"}]}'
----------------------------------------------------------
CREATE OR ALTER PROCEDURE AddProduct(@jsonData NVARCHAR(MAX))
AS
BEGIN
	--Insert data into the Products table
	INSERT INTO Instances.Products(
		Name
		,Description,
		ProductImageUris
		,ValidSkus
		,CreatedTimeStamp
		)
	SELECT JSON_VALUE(@jsonData, '$.name') AS Name
		,JSON_VALUE(@jsonData, '$.description') AS Description
		,JSON_VALUE(@jsonData, '$.productimageuris') AS ProductImageUris
		,JSON_VALUE(@jsonData, '$.validskus') AS ValidSkus
		,GETDATE();

	-- Insert data into the ProductAttributes table
	DECLARE @productId INT;

	SET @productId = SCOPE_IDENTITY();-- Get the last inserted productId

	INSERT INTO Instances.ProductAttributes (
		InstanceId
		,[Key]
		,[Value]
		)
	SELECT @productId, [Key], [Value]
	FROM OPENJSON(@jsonData, '$.attributes') WITH 
	([Key] VARCHAR(64) '$.key', [Value] VARCHAR(512) '$.value');

	--Get first Category record that matches
	DECLARE @categoryId INT;

	SELECT @categoryId = InstanceId FROM Instances.Categories
	WHERE Name = JSON_VALUE(@jsonData, '$.categoryname')

	-- Insert data into the ProductCategories table
	INSERT INTO Instances.ProductCategories (
		InstanceId
		,CategoryInstanceId
		)
	SELECT @productId, @categoryId
END
