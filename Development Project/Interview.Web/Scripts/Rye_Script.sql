USE [SparcInventoryDB]
GO
/****** Object:  Schema [Client]    Script Date: 3/15/2024 5:23:48 PM ******/
CREATE SCHEMA [Client]
GO
/****** Object:  Table [Client].[Clients]    Script Date: 3/15/2024 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Client].[Clients](
	[ClientId] [int] IDENTITY(1,1) NOT NULL,
	[ClientName] [varchar](50) NOT NULL,
	[CreatedTimeStamp] [datetime] NOT NULL,
 CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED 
(
	[ClientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Instances].[Products]    Script Date: 3/15/2024 5:23:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Instances].[Products](
	[InstanceId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](256) NOT NULL,
	[Description] [varchar](256) NOT NULL,
	[ProductImageUris] [varchar](max) NOT NULL,
	[ValidSkus] [varchar](max) NOT NULL,
	[CreatedTimestamp] [datetime2](7) NOT NULL,
	[ClientId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[InstanceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [Client].[Clients] ON 
GO
INSERT [Client].[Clients] ([ClientId], [ClientName], [CreatedTimeStamp]) VALUES (1, N'Walmart', CAST(N'2024-03-14T16:18:24.957' AS DateTime))
GO
SET IDENTITY_INSERT [Client].[Clients] OFF
GO
SET IDENTITY_INSERT [Instances].[Products] ON 
GO
INSERT [Instances].[Products] ([InstanceId], [Name], [Description], [ProductImageUris], [ValidSkus], [CreatedTimestamp], [ClientId]) VALUES (6, N'Franklin Football', N'Franklin Sports Kids Junior Football - Grip-Rite 100 Youth Junior Size Rubber Footballs - Peewee Kids Durable Outdoor Rubber Footballs - Single + 6 Bulk Packs with Inflation Pump', N'https://m.media-amazon.com/images/I/91c3Ca9UdiS._AC_SX425_.jpg', N'4234jlr35e', CAST(N'2024-03-14T23:56:24.2266667' AS DateTime2), 1)
GO
INSERT [Instances].[Products] ([InstanceId], [Name], [Description], [ProductImageUris], [ValidSkus], [CreatedTimestamp], [ClientId]) VALUES (7, N'Franklin Sports', N'Franklin Sports Blackhawk Backyard Soccer Goal - Portable Pop Up Soccer Nets - Youth + Adult Folding Indoor + Outdoor Goals - Multiple Sizes + Colors - Perfect for Games + Practice', N'https://m.media-amazon.com/images/I/71ADSrI6enL._AC_SX569_.jpg" data-old-hires="https://m.media-amazon.com/images/I/71ADSrI6enL._AC_SL1024_.jpg', N'423894798378f', CAST(N'2024-03-15T00:01:17.8333333' AS DateTime2), 1)
GO
SET IDENTITY_INSERT [Instances].[Products] OFF
GO
ALTER TABLE [Instances].[Products] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedTimestamp]
GO
/****** Object:  StoredProcedure [dbo].[GetInventoryCount]    Script Date: 3/15/2024 5:23:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shawn Rye
-- Create date: 3/14/2024
-- Description:	Get Inventory count for all products, will leave Client ID out for now but can add it later
-- =============================================
CREATE PROCEDURE [dbo].[GetInventoryCount]
	
	@InstanceId INT
AS
BEGIN
	

SELECT  COUNT(*) AS ProductCount
FROM      Transactions.InventoryTransactions
WHERE   (ProductInstanceId = @InstanceId)
GROUP BY ProductInstanceId




END
GO
/****** Object:  StoredProcedure [dbo].[GetProducts]    Script Date: 3/15/2024 5:23:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shawn Rye
-- Create date: 3/14/2024
-- Description:	Gets product information based off of the clientID
-- =============================================
CREATE PROCEDURE [dbo].[GetProducts]
	
	@ClientId INT,
	@ProductName Varchar(56)
AS
BEGIN
	
	SET NOCOUNT ON;
	


SELECT   Instances.Products.InstanceId, Instances.Products.Name, Instances.Products.Description, Instances.Products.ProductImageUris, Instances.Products.ValidSkus, Instances.Products.CreatedTimestamp, Instances.ProductAttributes.[Key], 
                Instances.ProductAttributes.Value AS ProductAttributeValue, Instances.Categories.Name AS CategoryName, Instances.Categories.Description AS CategoryDescription, Instances.ProductCategories.CategoryInstanceId,  
				Instances.CategoryAttributes.Value AS CategoryAttributeValue,
                Transactions.InventoryTransactions.TransactionId, Transactions.InventoryTransactions.ProductInstanceId, Transactions.InventoryTransactions.Quantity, Transactions.InventoryTransactions.StartedTimestamp, 
                Transactions.InventoryTransactions.TypeCategory
FROM      Instances.Products INNER JOIN
                Instances.ProductCategories ON Instances.Products.InstanceId = Instances.ProductCategories.InstanceId INNER JOIN
                Instances.ProductAttributes ON Instances.Products.InstanceId = Instances.ProductAttributes.InstanceId INNER JOIN
                Instances.Categories ON Instances.Products.InstanceId = Instances.Categories.InstanceId INNER JOIN
                Instances.CategoryAttributes ON Instances.ProductAttributes.[Key] = Instances.CategoryAttributes.[Key] INNER JOIN
                Instances.CategoryCategories ON Instances.ProductCategories.CategoryInstanceId = Instances.CategoryCategories.CategoryInstanceId INNER JOIN
                Transactions.InventoryTransactions ON Instances.Products.InstanceId = Transactions.InventoryTransactions.ProductInstanceId
				WHERE Instances.Products.ClientId = @ClientId and Instances.Products.Name Like '%' + @ProductName + '%';
END
GO
/****** Object:  StoredProcedure [dbo].[InsertInventoryTransactions]    Script Date: 3/15/2024 5:23:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shawn Rye
-- Create date: 3/14/2024
-- Description:	Insert Transactions
-- =============================================
CREATE PROCEDURE [dbo].[InsertInventoryTransactions]
	
	@ClientId INT,
    @InstanceId INT,
	@Quantity Decimal,
	@TypeCategory varchar(32)
	   
AS
BEGIN

	
	
	INSERT INTO [Transactions].[InventoryTransactions](ProductInstanceId,Quantity,StartedTimestamp,CompletedTimestamp, TypeCategory)
	VALUES(@InstanceId,@Quantity,GETDATE(),GETDATE(),@TypeCategory)

	

END
GO
/****** Object:  StoredProcedure [dbo].[RemoveInventoryTransactions]    Script Date: 3/15/2024 5:23:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shawn Rye
-- Create date: 3/14/2024
-- Description:	Remove Transactions
-- =============================================
CREATE PROCEDURE [dbo].[RemoveInventoryTransactions]
	
	@TransactionID INT
  	   
AS
BEGIN

	DELETE FROM [Transactions].[InventoryTransactions]
	WHERE TransactionId = @TransactionID
	

END
GO
