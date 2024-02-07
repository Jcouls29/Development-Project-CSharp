CREATE TABLE [Instances].[ProductCategories]
(
	[InstanceId] INT NOT NULL,
	[CategoryInstanceId] INT NOT NULL, 
    CONSTRAINT [PK_ProductCategories] PRIMARY KEY ([InstanceId], [CategoryInstanceId]), 
    CONSTRAINT [FK_ProductCategories_Products] FOREIGN KEY ([InstanceId]) REFERENCES [Instances].[Products]([InstanceId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProductCategories_Categories] FOREIGN KEY ([CategoryInstanceId]) REFERENCES [Instances].[Categories]([InstanceId]) ON DELETE CASCADE
)
