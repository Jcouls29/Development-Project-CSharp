IF DB_ID(N'InterviewProject') IS NOT NULL
BEGIN
    ALTER DATABASE [InterviewProject] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [InterviewProject];
END
GO

CREATE DATABASE [InterviewProject];
GO

USE [InterviewProject];
GO

:r .\Sparcpoint.Inventory.Database\Instances\Instances.sql
GO
:r .\Sparcpoint.Inventory.Database\Instances\Tables\Categories.sql
GO
:r .\Sparcpoint.Inventory.Database\Instances\Tables\Products.sql
GO
:r .\Sparcpoint.Inventory.Database\Instances\Tables\ProductAttributes.sql
GO
:r .\Sparcpoint.Inventory.Database\Instances\Tables\CategoryAttributes.sql
GO
:r .\Sparcpoint.Inventory.Database\Instances\Tables\ProductCategories.sql
GO
:r .\Sparcpoint.Inventory.Database\Instances\Tables\CategoryCategories.sql
GO
:r .\Sparcpoint.Inventory.Database\Transactions\Transactions.sql
GO
