CREATE TABLE [Instances].[Product] (
    [ProductId]        INT            IDENTITY (1, 1) NOT NULL,
    [Manufacturer]     VARCHAR (256)  NOT NULL,
    [ModelName]        VARCHAR (256)  NOT NULL,
    [Description]      VARCHAR (256)  NOT NULL,
    [CreatedTimestamp] DATETIME2 (7)  CONSTRAINT [DF__Product__Created__37A5467C] DEFAULT (sysutcdatetime()) NOT NULL,
    [CategoriesJson]   VARCHAR (1000) NOT NULL,
    CONSTRAINT [PK__Product__B40CC6CDA9B7B03D] PRIMARY KEY CLUSTERED ([ProductId] ASC)
);






GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Manufacturer_Brand]
    ON [Instances].[Product]([Manufacturer] ASC, [ModelName] ASC);

