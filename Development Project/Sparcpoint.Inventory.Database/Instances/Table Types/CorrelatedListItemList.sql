CREATE TYPE [Instances].[CorrelatedListItemList] AS TABLE
(
	[Index] INT NOT NULL,
	[InstanceId] INT NULL,
	[Name] VARCHAR(64) NOT NULL,
	[Description] VARCHAR(256) NOT NULL
)
