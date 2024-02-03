CREATE TABLE [Instances].[Product] (
    [ProductId]        INT           IDENTITY (1, 1) NOT NULL,
    [Manufacturer]     VARCHAR (256) NOT NULL,
    [BrandName]        VARCHAR (256) NOT NULL,
    [Description]      VARCHAR (256) NOT NULL,
    [CreatedTimestamp] DATETIME2 (7) DEFAULT (sysutcdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([ProductId] ASC)
);

