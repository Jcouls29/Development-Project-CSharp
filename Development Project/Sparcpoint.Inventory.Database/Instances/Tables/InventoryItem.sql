CREATE TABLE [Instances].[InventoryItem] (
    [Sku]              VARCHAR (10)  NOT NULL,
    [ProductId]        INT           NOT NULL,
    [AttributesJson]   VARCHAR (MAX) NOT NULL,
    [QuantityOnHand]   INT           NOT NULL,
    [CreatedTimestamp] DATETIME2 (7) DEFAULT (sysutcdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Sku] ASC),
    CONSTRAINT [FK_InventoryItem_Product] FOREIGN KEY ([ProductId]) REFERENCES [Instances].[Product] ([ProductId]) ON DELETE CASCADE ON UPDATE CASCADE
);



