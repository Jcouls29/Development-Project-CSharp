:on error exit

IF DB_ID(N'inventory') IS NULL
BEGIN
    CREATE DATABASE [inventory];
END
GO

USE [inventory];
GO

:r ".\Development Project\Sparcpoint.Inventory.Database\Instances\Instances.sql"
:r ".\Development Project\Sparcpoint.Inventory.Database\Transactions\Transactions.sql"

:r ".\Development Project\Sparcpoint.Inventory.Database\Table Types\IntegerList.sql"
:r ".\Development Project\Sparcpoint.Inventory.Database\Table Types\StringList.sql"
:r ".\Development Project\Sparcpoint.Inventory.Database\Table Types\CustomAttributeList.sql"
:r ".\Development Project\Sparcpoint.Inventory.Database\Table Types\CorrelatedIntegerList.sql"
:r ".\Development Project\Sparcpoint.Inventory.Database\Table Types\CorrelatedStringList.sql"
:r ".\Development Project\Sparcpoint.Inventory.Database\Table Types\CorrelatedCustomAttributeList.sql"
:r ".\Development Project\Sparcpoint.Inventory.Database\Instances\Table Types\CorrelatedListItemList.sql"
:r ".\Development Project\Sparcpoint.Inventory.Database\Instances\Table Types\CorrelatedProductInstanceList.sql"

:r ".\Development Project\Sparcpoint.Inventory.Database\Instances\Tables\Categories.sql"
:r ".\Development Project\Sparcpoint.Inventory.Database\Instances\Tables\Products.sql"
:r ".\Development Project\Sparcpoint.Inventory.Database\Instances\Tables\CategoryAttributes.sql"
:r ".\Development Project\Sparcpoint.Inventory.Database\Instances\Tables\ProductAttributes.sql"
:r ".\Development Project\Sparcpoint.Inventory.Database\Instances\Tables\ProductCategories.sql"
:r ".\Development Project\Sparcpoint.Inventory.Database\Instances\Tables\CategoryCategories.sql"
:r ".\Development Project\Sparcpoint.Inventory.Database\Transactions\Tables\InventoryTransactions.sql"
