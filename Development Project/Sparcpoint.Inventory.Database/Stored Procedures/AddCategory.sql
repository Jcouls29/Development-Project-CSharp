----------------------------------------------------------
--Created By: Jeremy Hill 20230729
--Purpose: To Add New Categories to the system
--Note:  Formatted using Poor Man's T-SQL formatter
--Example: 
--AddCategory '{"name": "Category XYZ","description": "This is the xyz category","attributes": [{"key": "Color","value": "Blue"},{"key": "Material","value": "Cotton"}]}'
----------------------------------------------------------
CREATE OR ALTER PROCEDURE AddCategory(@jsonData NVARCHAR(MAX))
AS
BEGIN
	--Insert data into the Categories table
	INSERT INTO Instances.Categories (
		Name
		,Description
		,CreatedTimeStamp
		)
	SELECT JSON_VALUE(@jsonData, '$.name') AS Name
		,JSON_VALUE(@jsonData, '$.description') AS Description
		,GETDATE();

	-- Insert data into the CategoryAttributes table
	DECLARE @categoryId INT;

	SET @categoryId = SCOPE_IDENTITY();-- Get the last inserted CategoryId

	INSERT INTO Instances.CategoryAttributes (
		InstanceId
		,[Key]
		,[Value]
		)
	SELECT @categoryId, [Key], [Value]
	FROM OPENJSON(@jsonData, '$.attributes') WITH 
	([Key] VARCHAR(64) '$.key', [Value] VARCHAR(512) '$.value');

	-- Insert data into the CategoryCategories table
	--INSERT INTO Instances.CategoryCategories (
	--	InstanceId
	--	,CategoryInstanceId
	--	)
	--SELECT InstanceId, @categoryId FROM Instances.CategoryAttributes WHERE
	--InstanceId NOT IN (SELECT InstanceId FROM Instances.CategoryCategories)
END