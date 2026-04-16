CREATE TABLE [Transactions].[InventoryTransactions]
(
	[TransactionId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[ProductInstanceId] INT NOT NULL,
	[Quantity] DECIMAL(19,6) NOT NULL,
	[StartedTimestamp] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
	[CompletedTimestamp] DATETIME2(7) NULL,
	[TypeCategory] VARCHAR(32) NULL,
	/* EVAL: Soft Delete transaction, might be better */
	[IsDeleted] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [FK_InventoryTransactions_Products] FOREIGN KEY ([ProductInstanceId]) REFERENCES [Instances].[Products]([InstanceId]) ON DELETE CASCADE
)

GO

CREATE INDEX [IX_InventoryTransactions_ProductInstanceId] ON [Transactions].[InventoryTransactions] ([ProductInstanceId])

GO

CREATE INDEX [IX_InventoryTransactions_ProductInstanceId_Quantity] ON [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity])

GO

CREATE INDEX [IX_InventoryTransactions_CompletedTimestamp] ON [Transactions].[InventoryTransactions] ([CompletedTimestamp])

GO
