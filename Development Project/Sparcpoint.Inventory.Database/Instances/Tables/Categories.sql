CREATE TABLE [Instances].[Categories]
(
	[InstanceId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] VARCHAR(64) NOT NULL,
	[Description] VARCHAR(256) NOT NULL,
	[CreatedTimestamp] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(), 
)

GO