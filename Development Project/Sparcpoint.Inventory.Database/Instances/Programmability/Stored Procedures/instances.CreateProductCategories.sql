CREATE PROCEDURE [instances].[CreateProductCategories]
	@InstanceId INT,
	@CategoryInstanceId INT
AS

Begin
		insert into [Instances].ProductCategories(InstanceId, CategoryInstanceId) values(@InstanceId, @CategoryInstanceId);
		insert into [Instances].CategoryCategories(InstanceId, CategoryInstanceId) values(@CategoryInstanceId, @CategoryInstanceId)
END
RETURN
