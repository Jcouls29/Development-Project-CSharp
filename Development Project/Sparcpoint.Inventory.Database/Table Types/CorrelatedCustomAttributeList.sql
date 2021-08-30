CREATE TYPE [dbo].[CorrelatedCustomAttributeList] AS TABLE
(
	[Index] INT NOT NULL,
	[Key] VARCHAR(64) NOT NULL,
	[Value] VARCHAR(512) NOT NULL
)
