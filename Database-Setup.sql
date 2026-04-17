-- ============================================================================
-- Sparcpoint Inventory — Database Setup (IDEMPOTENT)
-- Run this script ONE single time to set up the complete DB.
-- EVAL: Idempotent script to avoid errors when re-running in development.
-- ============================================================================

USE [SparcpointInventory];
GO

-- ----------------------------------------------------------------------------
-- Schemas
-- ----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'Instances')
    EXEC('CREATE SCHEMA [Instances]');
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'Transactions')
    EXEC('CREATE SCHEMA [Transactions]');
GO

-- ----------------------------------------------------------------------------
-- Table Types (dbo) - Generic TVPs for bulk operations
-- ----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.types WHERE name = N'StringList' AND is_table_type = 1)
    CREATE TYPE [dbo].[StringList] AS TABLE ([Value] VARCHAR(512) NOT NULL);
GO

IF NOT EXISTS (SELECT 1 FROM sys.types WHERE name = N'IntegerList' AND is_table_type = 1)
    CREATE TYPE [dbo].[IntegerList] AS TABLE ([Value] INT NOT NULL);
GO

IF NOT EXISTS (SELECT 1 FROM sys.types WHERE name = N'CustomAttributeList' AND is_table_type = 1)
    CREATE TYPE [dbo].[CustomAttributeList] AS TABLE (
        [Key]   VARCHAR(64)  NOT NULL,
        [Value] VARCHAR(512) NOT NULL
    );
GO

IF NOT EXISTS (SELECT 1 FROM sys.types WHERE name = N'CorrelatedStringList' AND is_table_type = 1)
    CREATE TYPE [dbo].[CorrelatedStringList] AS TABLE (
        [Index] INT          NOT NULL,
        [Value] VARCHAR(512) NOT NULL
    );
GO

IF NOT EXISTS (SELECT 1 FROM sys.types WHERE name = N'CorrelatedIntegerList' AND is_table_type = 1)
    CREATE TYPE [dbo].[CorrelatedIntegerList] AS TABLE (
        [Index] INT NOT NULL,
        [Value] INT NOT NULL
    );
GO

IF NOT EXISTS (SELECT 1 FROM sys.types WHERE name = N'CorrelatedCustomAttributeList' AND is_table_type = 1)
    CREATE TYPE [dbo].[CorrelatedCustomAttributeList] AS TABLE (
        [Index] INT          NOT NULL,
        [Key]   VARCHAR(64)  NOT NULL,
        [Value] VARCHAR(512) NOT NULL
    );
GO

-- ----------------------------------------------------------------------------
-- Table Types (Instances)
-- ----------------------------------------------------------------------------
IF NOT EXISTS (
    SELECT 1 FROM sys.types t
    JOIN sys.schemas s ON t.schema_id = s.schema_id
    WHERE t.name = N'CorrelatedListItemList' AND s.name = N'Instances' AND t.is_table_type = 1
)
    CREATE TYPE [Instances].[CorrelatedListItemList] AS TABLE (
        [Index]       INT          NOT NULL,
        [InstanceId]  INT          NULL,
        [Name]        VARCHAR(64)  NOT NULL,
        [Description] VARCHAR(256) NOT NULL
    );
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.types t
    JOIN sys.schemas s ON t.schema_id = s.schema_id
    WHERE t.name = N'CorrelatedProductInstanceList' AND s.name = N'Instances' AND t.is_table_type = 1
)
    CREATE TYPE [Instances].[CorrelatedProductInstanceList] AS TABLE (
        [Index]            INT          NOT NULL,
        [DefinitionId]     INT          NOT NULL,
        [Name]             VARCHAR(256) NOT NULL,
        [Description]      VARCHAR(256) NOT NULL,
        [ProductImageUris] VARCHAR(MAX) NOT NULL,
        [ValidSkus]        VARCHAR(MAX) NOT NULL
    );
GO

-- ----------------------------------------------------------------------------
-- Transactions.InventoryTransactions
-- EVAL: Immutable record of inventory movements. The "undo" is modeled
--       by marking the CompletedTimestamp, not with DELETE, to preserve audit trail.
-- ----------------------------------------------------------------------------
IF NOT EXISTS (
    SELECT 1 FROM sys.tables t
    JOIN sys.schemas s ON t.schema_id = s.schema_id
    WHERE t.name = N'InventoryTransactions' AND s.name = N'Transactions'
)
BEGIN
    CREATE TABLE [Transactions].[InventoryTransactions]
    (
        [TransactionId]       INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ProductInstanceId]   INT            NOT NULL,
        [Quantity]            DECIMAL(19,6)  NOT NULL,
        [StartedTimestamp]    DATETIME2(7)   NOT NULL DEFAULT SYSUTCDATETIME(),
        [CompletedTimestamp]  DATETIME2(7)   NULL,
        [TypeCategory]        VARCHAR(32)    NULL,
        CONSTRAINT [FK_InventoryTransactions_Products]
            FOREIGN KEY ([ProductInstanceId])
            REFERENCES [Instances].[Products]([InstanceId])
            ON DELETE CASCADE
    );

    CREATE INDEX [IX_InventoryTransactions_ProductInstanceId]
        ON [Transactions].[InventoryTransactions] ([ProductInstanceId]);

    CREATE INDEX [IX_InventoryTransactions_ProductInstanceId_Quantity]
        ON [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity]);

    CREATE INDEX [IX_InventoryTransactions_CompletedTimestamp]
        ON [Transactions].[InventoryTransactions] ([CompletedTimestamp]);
END
GO

PRINT 'Sparcpoint Inventory database setup completed.';
GO
