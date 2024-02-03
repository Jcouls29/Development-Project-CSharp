CREATE TABLE [Instances].[CategoryAttributes]
(
	[InstanceId] INT NOT NULL,
	[Key] VARCHAR(64) NOT NULL,
	[Value] VARCHAR(512) NOT NULL, 
    CONSTRAINT [PK_CategoryAttributes] PRIMARY KEY ([InstanceId], [Key]), 
    CONSTRAINT [FK_CategoryAttributes_Categories] FOREIGN KEY ([InstanceId]) REFERENCES [Instances].[Categories]([InstanceId]) ON DELETE CASCADE
)
