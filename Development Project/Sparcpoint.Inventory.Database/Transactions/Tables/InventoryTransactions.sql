

GO

CREATE INDEX [IX_InventoryTransactions_ProductInstanceId] ON [Transactions].[InventoryTransactions] ([ProductInstanceId])

GO

CREATE INDEX [IX_InventoryTransactions_ProductInstanceId_Quantity] ON [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity])

GO

CREATE INDEX [IX_InventoryTransactions_CompletedTimestamp] ON [Transactions].[InventoryTransactions] ([CompletedTimestamp])

GO
