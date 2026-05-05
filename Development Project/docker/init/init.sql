-- =============================================================================
-- Sparcpoint Inventory Database - Idempotent Initialization Script
-- Safe to run multiple times. Creates objects only if they don't exist.
-- Seed data is inserted only into empty tables.
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 1. Create Database
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'SparcpointInventory')
BEGIN
    CREATE DATABASE [SparcpointInventory];
    PRINT 'Created database SparcpointInventory';
END
GO

USE [SparcpointInventory];
GO

-- -----------------------------------------------------------------------------
-- 2. Create Schemas
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Instances')
    EXEC('CREATE SCHEMA [Instances]');
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Transactions')
    EXEC('CREATE SCHEMA [Transactions]');
GO

-- -----------------------------------------------------------------------------
-- 3. Create Table Types
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.types WHERE name = N'CustomAttributeList' AND schema_id = SCHEMA_ID('dbo'))
    CREATE TYPE [dbo].[CustomAttributeList] AS TABLE
    (
        [Key] VARCHAR(64) NOT NULL,
        [Value] VARCHAR(512) NOT NULL
    );
GO

IF NOT EXISTS (SELECT * FROM sys.types WHERE name = N'IntegerList' AND schema_id = SCHEMA_ID('dbo'))
    CREATE TYPE [dbo].[IntegerList] AS TABLE
    (
        [Value] INT NOT NULL
    );
GO

IF NOT EXISTS (SELECT * FROM sys.types WHERE name = N'StringList' AND schema_id = SCHEMA_ID('dbo'))
    CREATE TYPE [dbo].[StringList] AS TABLE
    (
        [Value] VARCHAR(512) NOT NULL
    );
GO

IF NOT EXISTS (SELECT * FROM sys.types WHERE name = N'CorrelatedCustomAttributeList' AND schema_id = SCHEMA_ID('dbo'))
    CREATE TYPE [dbo].[CorrelatedCustomAttributeList] AS TABLE
    (
        [Index] INT NOT NULL,
        [Key] VARCHAR(64) NOT NULL,
        [Value] VARCHAR(512) NOT NULL
    );
GO

IF NOT EXISTS (SELECT * FROM sys.types WHERE name = N'CorrelatedIntegerList' AND schema_id = SCHEMA_ID('dbo'))
    CREATE TYPE [dbo].[CorrelatedIntegerList] AS TABLE
    (
        [Index] INT NOT NULL,
        [Value] INT NOT NULL
    );
GO

IF NOT EXISTS (SELECT * FROM sys.types WHERE name = N'CorrelatedStringList' AND schema_id = SCHEMA_ID('dbo'))
    CREATE TYPE [dbo].[CorrelatedStringList] AS TABLE
    (
        [Index] INT NOT NULL,
        [Value] VARCHAR(512) NOT NULL
    );
GO

IF NOT EXISTS (SELECT * FROM sys.types WHERE name = N'CorrelatedListItemList' AND schema_id = SCHEMA_ID('Instances'))
    CREATE TYPE [Instances].[CorrelatedListItemList] AS TABLE
    (
        [Index] INT NOT NULL,
        [InstanceId] INT NULL,
        [Name] VARCHAR(64) NOT NULL,
        [Description] VARCHAR(256) NOT NULL
    );
GO

IF NOT EXISTS (SELECT * FROM sys.types WHERE name = N'CorrelatedProductInstanceList' AND schema_id = SCHEMA_ID('Instances'))
    CREATE TYPE [Instances].[CorrelatedProductInstanceList] AS TABLE
    (
        [Index] INT NOT NULL,
        [DefinitionId] INT NOT NULL,
        [Name] VARCHAR(256) NOT NULL,
        [Description] VARCHAR(256) NOT NULL,
        [ProductImageUris] VARCHAR(MAX) NOT NULL,
        [ValidSkus] VARCHAR(MAX) NOT NULL
    );
GO

-- -----------------------------------------------------------------------------
-- 4. Create Tables (in FK dependency order)
-- -----------------------------------------------------------------------------

-- Categories (no FK dependencies)
IF OBJECT_ID('Instances.Categories', 'U') IS NULL
    CREATE TABLE [Instances].[Categories]
    (
        [InstanceId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [Name] VARCHAR(64) NOT NULL,
        [Description] VARCHAR(256) NOT NULL,
        [CreatedTimestamp] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()
    );
GO

-- CategoryAttributes (FK -> Categories)
IF OBJECT_ID('Instances.CategoryAttributes', 'U') IS NULL
    CREATE TABLE [Instances].[CategoryAttributes]
    (
        [InstanceId] INT NOT NULL,
        [Key] VARCHAR(64) NOT NULL,
        [Value] VARCHAR(512) NOT NULL,
        CONSTRAINT [PK_CategoryAttributes] PRIMARY KEY ([InstanceId], [Key]),
        CONSTRAINT [FK_CategoryAttributes_Categories] FOREIGN KEY ([InstanceId]) REFERENCES [Instances].[Categories]([InstanceId]) ON DELETE CASCADE
    );
GO

-- CategoryCategories (FK -> Categories x2, enables hierarchy)
IF OBJECT_ID('Instances.CategoryCategories', 'U') IS NULL
    CREATE TABLE [Instances].[CategoryCategories]
    (
        [InstanceId] INT NOT NULL,
        [CategoryInstanceId] INT NOT NULL,
        CONSTRAINT [PK_CategoryCategories] PRIMARY KEY ([InstanceId], [CategoryInstanceId]),
        CONSTRAINT [FK_CategoryCategories_Categories] FOREIGN KEY ([InstanceId]) REFERENCES [Instances].[Categories]([InstanceId]) ON DELETE CASCADE,
        CONSTRAINT [FK_CategoryCategories_Categories_Categories] FOREIGN KEY ([CategoryInstanceId]) REFERENCES [Instances].[Categories]([InstanceId])
    );
GO

-- Products (no FK dependencies)
IF OBJECT_ID('Instances.Products', 'U') IS NULL
    CREATE TABLE [Instances].[Products]
    (
        [InstanceId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [Name] VARCHAR(256) NOT NULL,
        [Description] VARCHAR(256) NOT NULL,
        [ProductImageUris] VARCHAR(MAX) NOT NULL,
        [ValidSkus] VARCHAR(MAX) NOT NULL,
        [CreatedTimestamp] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()
    );
GO

-- ProductAttributes (FK -> Products)
IF OBJECT_ID('Instances.ProductAttributes', 'U') IS NULL
BEGIN
    CREATE TABLE [Instances].[ProductAttributes]
    (
        [InstanceId] INT NOT NULL,
        [Key] VARCHAR(64) NOT NULL,
        [Value] VARCHAR(512) NOT NULL,
        CONSTRAINT [PK_ProductAttributes] PRIMARY KEY ([InstanceId], [Key]),
        CONSTRAINT [FK_ProductAttributes_Products] FOREIGN KEY ([InstanceId]) REFERENCES [Instances].[Products]([InstanceId]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_ProductAttributes_Key_Value] ON [Instances].[ProductAttributes] ([Key] ASC, [Value] ASC);
END
GO

-- ProductCategories (FK -> Products, Categories)
IF OBJECT_ID('Instances.ProductCategories', 'U') IS NULL
    CREATE TABLE [Instances].[ProductCategories]
    (
        [InstanceId] INT NOT NULL,
        [CategoryInstanceId] INT NOT NULL,
        CONSTRAINT [PK_ProductCategories] PRIMARY KEY ([InstanceId], [CategoryInstanceId]),
        CONSTRAINT [FK_ProductCategories_Products] FOREIGN KEY ([InstanceId]) REFERENCES [Instances].[Products]([InstanceId]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProductCategories_Categories] FOREIGN KEY ([CategoryInstanceId]) REFERENCES [Instances].[Categories]([InstanceId]) ON DELETE CASCADE
    );
GO

-- InventoryTransactions (FK -> Products)
IF OBJECT_ID('Transactions.InventoryTransactions', 'U') IS NULL
BEGIN
    CREATE TABLE [Transactions].[InventoryTransactions]
    (
        [TransactionId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ProductInstanceId] INT NOT NULL,
        [Quantity] DECIMAL(19,6) NOT NULL,
        [StartedTimestamp] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        [CompletedTimestamp] DATETIME2(7) NULL,
        [TypeCategory] VARCHAR(32) NULL,
        CONSTRAINT [FK_InventoryTransactions_Products] FOREIGN KEY ([ProductInstanceId]) REFERENCES [Instances].[Products]([InstanceId]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_InventoryTransactions_ProductInstanceId] ON [Transactions].[InventoryTransactions] ([ProductInstanceId]);
    CREATE INDEX [IX_InventoryTransactions_ProductInstanceId_Quantity] ON [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity]);
    CREATE INDEX [IX_InventoryTransactions_CompletedTimestamp] ON [Transactions].[InventoryTransactions] ([CompletedTimestamp]);
END
GO

-- -----------------------------------------------------------------------------
-- 5. Seed Data (only if tables are empty)
-- -----------------------------------------------------------------------------

-- Seed Categories
IF NOT EXISTS (SELECT 1 FROM [Instances].[Categories])
BEGIN
    PRINT 'Seeding categories...';

    SET IDENTITY_INSERT [Instances].[Categories] ON;

    -- Top-level categories
    INSERT INTO [Instances].[Categories] ([InstanceId], [Name], [Description])
    VALUES
        (1, 'Electronics',     'Electronic devices and accessories'),
        (2, 'Clothing',        'Apparel and fashion items'),
        (3, 'Home & Garden',   'Home improvement and garden supplies');

    -- Sub-categories
    INSERT INTO [Instances].[Categories] ([InstanceId], [Name], [Description])
    VALUES
        (4, 'Phones',          'Mobile phones and smartphones'),
        (5, 'Laptops',         'Laptop computers and notebooks'),
        (6, 'Shoes',           'Footwear for all occasions'),
        (7, 'Outerwear',       'Jackets, coats, and outer layers'),
        (8, 'Power Tools',     'Electric and battery-powered tools');

    SET IDENTITY_INSERT [Instances].[Categories] OFF;

    -- Category hierarchy (child -> parent)
    INSERT INTO [Instances].[CategoryCategories] ([InstanceId], [CategoryInstanceId])
    VALUES
        (4, 1),  -- Phones -> Electronics
        (5, 1),  -- Laptops -> Electronics
        (6, 2),  -- Shoes -> Clothing
        (7, 2),  -- Outerwear -> Clothing
        (8, 3);  -- Power Tools -> Home & Garden

    -- Category attributes
    INSERT INTO [Instances].[CategoryAttributes] ([InstanceId], [Key], [Value])
    VALUES
        (1, 'Department',   'Technology'),
        (2, 'Department',   'Fashion'),
        (3, 'Department',   'Home'),
        (4, 'Warranty',     '2 Years'),
        (5, 'Warranty',     '1 Year');
END
GO

-- Seed Products
IF NOT EXISTS (SELECT 1 FROM [Instances].[Products])
BEGIN
    PRINT 'Seeding products...';

    SET IDENTITY_INSERT [Instances].[Products] ON;

    INSERT INTO [Instances].[Products] ([InstanceId], [Name], [Description], [ProductImageUris], [ValidSkus])
    VALUES
        (1, 'Galaxy S24 Ultra',       'Samsung flagship smartphone',          '["https://example.com/images/galaxy-s24.jpg"]',         '["SM-S928BZKDEUB"]'),
        (2, 'MacBook Pro 16"',        'Apple laptop with M3 Pro chip',        '["https://example.com/images/macbook-pro-16.jpg"]',     '["MRW13LL/A","MRW23LL/A"]'),
        (3, 'Air Max 90',             'Nike classic running shoe',            '["https://example.com/images/airmax90.jpg"]',           '["CN8490-001","CN8490-002"]'),
        (4, 'DeWalt 20V MAX Drill',   'Cordless drill/driver kit',            '["https://example.com/images/dewalt-drill.jpg"]',       '["DCD771C2"]'),
        (5, 'North Face Thermoball',  'Lightweight insulated jacket',         '["https://example.com/images/thermoball.jpg"]',         '["NF0A5IDA-BLK","NF0A5IDA-NVY"]');

    SET IDENTITY_INSERT [Instances].[Products] OFF;

    -- Product attributes (arbitrary metadata)
    INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
    VALUES
        -- Galaxy S24 Ultra
        (1, 'Brand',        'Samsung'),
        (1, 'Color',        'Titanium Black'),
        (1, 'Storage',      '256GB'),
        (1, 'PackageUnit',  'Each'),
        -- MacBook Pro
        (2, 'Brand',        'Apple'),
        (2, 'Color',        'Space Black'),
        (2, 'Storage',      '512GB'),
        (2, 'PackageUnit',  'Each'),
        -- Air Max 90
        (3, 'Brand',        'Nike'),
        (3, 'Color',        'White/Black'),
        (3, 'Size',         '10'),
        (3, 'PackageUnit',  'Pair'),
        -- DeWalt Drill
        (4, 'Brand',        'DeWalt'),
        (4, 'Color',        'Yellow'),
        (4, 'Voltage',      '20V'),
        (4, 'PackageUnit',  'Kit'),
        -- Thermoball Jacket
        (5, 'Brand',        'The North Face'),
        (5, 'Color',        'Black'),
        (5, 'Size',         'L'),
        (5, 'PackageUnit',  'Each');

    -- Product -> Category associations
    INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
    VALUES
        (1, 1),  -- Galaxy -> Electronics
        (1, 4),  -- Galaxy -> Phones
        (2, 1),  -- MacBook -> Electronics
        (2, 5),  -- MacBook -> Laptops
        (3, 2),  -- Air Max -> Clothing
        (3, 6),  -- Air Max -> Shoes
        (4, 3),  -- DeWalt -> Home & Garden
        (4, 8),  -- DeWalt -> Power Tools
        (5, 2),  -- Thermoball -> Clothing
        (5, 7);  -- Thermoball -> Outerwear
END
GO

-- Seed Inventory Transactions
IF NOT EXISTS (SELECT 1 FROM [Transactions].[InventoryTransactions])
BEGIN
    PRINT 'Seeding inventory transactions...';

    -- Initial stock receipts
    INSERT INTO [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity], [TypeCategory])
    VALUES
        (1,  50.000000,  'Purchase'),   -- 50 Galaxy phones received
        (2,  25.000000,  'Purchase'),   -- 25 MacBooks received
        (3, 100.000000,  'Purchase'),   -- 100 pairs Air Max received
        (4,  75.000000,  'Purchase'),   -- 75 DeWalt drills received
        (5,  60.000000,  'Purchase');   -- 60 Thermoball jackets received

    -- Some sales (negative quantity = removal)
    INSERT INTO [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity], [TypeCategory])
    VALUES
        (1, -12.000000,  'Sale'),       -- 12 Galaxy phones sold
        (2,  -8.000000,  'Sale'),       -- 8 MacBooks sold
        (3, -35.000000,  'Sale'),       -- 35 pairs Air Max sold
        (5, -15.000000,  'Sale');       -- 15 jackets sold

    -- A return
    INSERT INTO [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity], [TypeCategory])
    VALUES
        (1,   3.000000,  'Return');     -- 3 Galaxy phones returned

    -- An undone transaction (CompletedTimestamp set = excluded from counts)
    INSERT INTO [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity], [CompletedTimestamp], [TypeCategory])
    VALUES
        (3, -10.000000, SYSUTCDATETIME(), 'Sale');  -- This sale was undone

    PRINT 'Seed data complete.';
    PRINT 'Expected inventory counts:';
    PRINT '  Galaxy S24 Ultra:      41 (50 - 12 + 3)';
    PRINT '  MacBook Pro 16":       17 (25 - 8)';
    PRINT '  Air Max 90:            65 (100 - 35, undone -10 excluded)';
    PRINT '  DeWalt 20V MAX Drill:  75 (75)';
    PRINT '  North Face Thermoball: 45 (60 - 15)';
END
GO
