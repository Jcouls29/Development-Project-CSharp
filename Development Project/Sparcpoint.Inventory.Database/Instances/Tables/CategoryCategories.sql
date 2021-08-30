CREATE TABLE [Instances].[CategoryCategories]
(
	[InstanceId] INT NOT NULL,
	[CategoryInstanceId] INT NOT NULL, 
    CONSTRAINT [PK_CategoryCategories] PRIMARY KEY ([InstanceId], [CategoryInstanceId]), 
    CONSTRAINT [FK_CategoryCategories_Categories] FOREIGN KEY ([InstanceId]) REFERENCES [Instances].[Categories]([InstanceId]) ON DELETE CASCADE, 
    CONSTRAINT [FK_CategoryCategories_Categories_Categories] FOREIGN KEY ([CategoryInstanceId]) REFERENCES [Instances].[Categories]([InstanceId])
)
