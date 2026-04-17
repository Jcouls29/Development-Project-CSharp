-- SQL script to create minimal tables used by the interview demo
-- Run in your SQL Server to prepare the database (InterviewDemo)

CREATE TABLE [dbo].[Products](
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(200) NOT NULL
);

CREATE TABLE [dbo].[ProductMetadata](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [MetaKey] NVARCHAR(200) NOT NULL,
    [MetaValue] NVARCHAR(MAX) NULL,
    CONSTRAINT FK_ProductMetadata_Product FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

CREATE TABLE [dbo].[ProductCategory](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [Category] NVARCHAR(200) NOT NULL,
    CONSTRAINT FK_ProductCategory_Product FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

CREATE TABLE [dbo].[InventoryTransaction](
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [Quantity] INT NOT NULL,
    [TransactionType] NVARCHAR(50) NOT NULL,
    [Timestamp] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    [Reversed] BIT NOT NULL DEFAULT 0,
    [RelatedTransactionId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT FK_InventoryTransaction_Product FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
