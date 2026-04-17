-- Migration: add Reversed and RelatedTransactionId to Transactions.InventoryTransactions
-- Run this against the database used by the app.

IF COL_LENGTH('[Transactions].[InventoryTransactions]', 'Reversed') IS NULL
BEGIN
    ALTER TABLE [Transactions].[InventoryTransactions]
    ADD [Reversed] BIT NOT NULL CONSTRAINT DF_InventoryTransactions_Reversed DEFAULT (0);
END

IF COL_LENGTH('[Transactions].[InventoryTransactions]', 'RelatedTransactionId') IS NULL
BEGIN
    ALTER TABLE [Transactions].[InventoryTransactions]
    ADD [RelatedTransactionId] INT NULL;
END

-- index to help lookups by ProductInstanceId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InventoryTransactions_RelatedTransactionId')
BEGIN
    CREATE INDEX [IX_InventoryTransactions_RelatedTransactionId] ON [Transactions].[InventoryTransactions] ([RelatedTransactionId]);
END
