CREATE TYPE [Instances].[CorrelatedProductInstanceList] AS TABLE
(
	[Index] INT NOT NULL,
	[DefinitionId] INT NOT NULL,
	[Name] VARCHAR(256) NOT NULL,
	[Description] VARCHAR(256) NOT NULL,
	[ProductImageUris] VARCHAR(MAX) NOT NULL,
	[ValidSkus] VARCHAR(MAX) NOT NULL
)
