----------------------------------------------------------
--Created By: Jeremy Hill 20230729
--Purpose: To Search for a Product(s) in the system
--Note:  Formatted using Poor Man's T-SQL formatter
--Example: 
--SearchProduct '{"category": "Category XYZ", "product": "product1", "productdescription": "This is the product1", "productattributes": [{"key": "size","value": "100mm"},{"key": "Manufacturer","value": "Acme"}]}'
----------------------------------------------------------
CREATE OR ALTER PROCEDURE SearchProduct(@jsonData NVARCHAR(MAX))
AS
BEGIN
	SELECT Name
	FROM Instances.Products
	WHERE InstanceId IN (
			SELECT InstanceId
			FROM Instances.Categories
			WHERE Name = JSON_VALUE(@jsonData, '$.category')
			)
		OR InstanceId IN (
			SELECT InstanceId
			FROM Instances.Products
			WHERE Name = JSON_VALUE(@jsonData, '$.product')
				OR Description = JSON_VALUE(@jsonData, '$.productdescription')
			)
		OR InstanceId IN (
			SELECT InstanceId
			FROM Instances.ProductAttributes
			WHERE [Key] IN (
					SELECT [Key]
					FROM OPENJSON(@jsonData, '$.attributes') WITH ([Key] VARCHAR(64) '$.key')
					)
				AND [Value] IN (
					SELECT [Key]
					FROM OPENJSON(@jsonData, '$.attributes') WITH ([Value] VARCHAR(512) '$.value')
					)
			)
	FOR JSON AUTO;
END
