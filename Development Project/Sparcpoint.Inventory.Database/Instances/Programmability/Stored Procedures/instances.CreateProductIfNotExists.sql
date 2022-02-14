CREATE PROCEDURE [instances].[CreateProductIfNotExists]
	@name VARCHAR (64),
	@description VARCHAR (256),
	@productImageUris VARCHAR (MAX), 
	@validSkus VARCHAR (MAX)
AS

Begin
	if not exists (select 'name' from instances.Products where name = @name)
		insert into instances.Products(Name, Description, ProductImageUris, ValidSkus) values(@name,@description,@productImageUris, @validSkus);
END
select Scope_identity()
