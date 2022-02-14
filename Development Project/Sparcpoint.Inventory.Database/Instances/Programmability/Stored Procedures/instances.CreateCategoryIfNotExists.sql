CREATE PROCEDURE [Instances].[CreateCategoryIfNotExists]
	@name VARCHAR (64),
	@description VARCHAR (256)
AS
Begin
	if not exists (select 'name' from Instances.Categories where name = @name)
		insert into Instances.Categories(Name, Description) values(@name,@description);
END
select Scope_identity()
