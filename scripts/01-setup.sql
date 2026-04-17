-- Create database
USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SparcpointInventory')
BEGIN
    CREATE DATABASE SparcpointInventory;
END
GO

USE SparcpointInventory;
GO

-- Create schemas (must be in separate batch)
EXEC('CREATE SCHEMA Instances');
GO

EXEC('CREATE SCHEMA Transactions');
GO

-- Create tables
CREATE TABLE [Instances].[Products](
    [InstanceId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Name] VARCHAR(256) NOT NULL,
    [Description] VARCHAR(256) NOT NULL,
    [ProductImageUris] VARCHAR(MAX) NOT NULL DEFAULT '[]',
    [ValidSkus] VARCHAR(MAX) NOT NULL DEFAULT '[]',
    [CreatedTimestamp] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE [Instances].[Categories](
    [InstanceId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Name] VARCHAR(64) NOT NULL,
    [Description] VARCHAR(256) NOT NULL,
    [CreatedTimestamp] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE [Instances].[ProductAttributes](
    [InstanceId] INT NOT NULL,
    [Key] VARCHAR(64) NOT NULL,
    [Value] VARCHAR(512) NOT NULL,
    CONSTRAINT [PK_ProductAttributes] PRIMARY KEY ([InstanceId], [Key])
);
GO

ALTER TABLE [Instances].[ProductAttributes] ADD CONSTRAINT [FK_PA_Products] 
    FOREIGN KEY ([InstanceId]) REFERENCES [Instances].[Products]([InstanceId]) ON DELETE CASCADE;
GO

CREATE TABLE [Instances].[ProductCategories](
    [InstanceId] INT NOT NULL,
    [CategoryInstanceId] INT NOT NULL,
    CONSTRAINT [PK_ProductCategories] PRIMARY KEY ([InstanceId], [CategoryInstanceId])
);
GO

ALTER TABLE [Instances].[ProductCategories] ADD CONSTRAINT [FK_PC_Products] 
    FOREIGN KEY ([InstanceId]) REFERENCES [Instances].[Products]([InstanceId]) ON DELETE CASCADE;
GO

ALTER TABLE [Instances].[ProductCategories] ADD CONSTRAINT [FK_PC_Categories] 
    FOREIGN KEY ([CategoryInstanceId]) REFERENCES [Instances].[Categories]([InstanceId]) ON DELETE CASCADE;
GO

CREATE TABLE [Instances].[CategoryAttributes](
    [InstanceId] INT NOT NULL,
    [Key] VARCHAR(64) NOT NULL,
    [Value] VARCHAR(512) NOT NULL,
    CONSTRAINT [PK_CategoryAttributes] PRIMARY KEY ([InstanceId], [Key])
);
GO

ALTER TABLE [Instances].[CategoryAttributes] ADD CONSTRAINT [FK_CA_Categories] 
    FOREIGN KEY ([InstanceId]) REFERENCES [Instances].[Categories]([InstanceId]) ON DELETE CASCADE;
GO

CREATE TABLE [Instances].[CategoryCategories](
    [InstanceId] INT NOT NULL,
    [CategoryInstanceId] INT NOT NULL,
    CONSTRAINT [PK_CategoryCategories] PRIMARY KEY ([InstanceId], [CategoryInstanceId])
);
GO

ALTER TABLE [Instances].[CategoryCategories] ADD CONSTRAINT [FK_CC_Child] 
    FOREIGN KEY ([InstanceId]) REFERENCES [Instances].[Categories]([InstanceId]) ON DELETE CASCADE;
GO

CREATE TABLE [Transactions].[InventoryTransactions](
    [TransactionId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ProductInstanceId] INT NOT NULL,
    [Quantity] DECIMAL(19,6) NOT NULL,
    [StartedTimestamp] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [CompletedTimestamp] DATETIME2(7) NULL,
    [TypeCategory] VARCHAR(32) NULL
);
GO

ALTER TABLE [Transactions].[InventoryTransactions] ADD CONSTRAINT [FK_IT_Products] 
    FOREIGN KEY ([ProductInstanceId]) REFERENCES [Instances].[Products]([InstanceId]) ON DELETE CASCADE;
GO

CREATE INDEX [IX_PA_Key_Value] ON [Instances].[ProductAttributes] ([Key], [Value]);
GO

CREATE INDEX [IX_CA_Key_Value] ON [Instances].[CategoryAttributes] ([Key], [Value]);
GO

CREATE INDEX [IX_IT_Product] ON [Transactions].[InventoryTransactions] ([ProductInstanceId]);
GO

CREATE INDEX [IX_IT_Completed] ON [Transactions].[InventoryTransactions] ([CompletedTimestamp]);
GO

PRINT 'Setup complete!';
GO