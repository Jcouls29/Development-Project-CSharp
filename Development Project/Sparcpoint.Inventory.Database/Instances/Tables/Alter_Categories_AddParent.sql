ALTER TABLE [Instances].[Categories]
ADD [ParentInstanceId] INT NULL;

GO

ALTER TABLE [Instances].[Categories]
ADD CONSTRAINT [FK_Categories_ParentCategory] FOREIGN KEY ([ParentInstanceId]) REFERENCES [Instances].[Categories]([InstanceId]) ON DELETE NO ACTION;

GO

CREATE INDEX [IX_Categories_ParentInstanceId] ON [Instances].[Categories] ([ParentInstanceId]);

GO