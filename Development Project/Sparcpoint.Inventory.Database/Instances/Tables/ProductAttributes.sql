CREATE TABLE [Instances].[ProductAttributes]
(
	[InstanceId] INT NOT NULL,
	[Key] VARCHAR(64) NOT NULL,
	[Value] VARCHAR(512) NOT NULL, 
    CONSTRAINT [PK_ProductAttributes] PRIMARY KEY ([InstanceId], [Key]), 
    CONSTRAINT [FK_ProductAttributes_Products] FOREIGN KEY ([InstanceId]) REFERENCES [Instances].[Products]([InstanceId]) ON DELETE CASCADE
)

GO

CREATE INDEX [IX_ProductAttributes_Key_Value] ON [Instances].[ProductAttributes] ([Key] ASC, [Value] ASC)
