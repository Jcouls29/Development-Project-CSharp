USE SparcpointInventory;
GO

IF NOT EXISTS (SELECT 1 FROM [Instances].[Categories] WHERE Name = 'Electronics')
    INSERT INTO [Instances].[Categories] ([Name], [Description]) VALUES ('Electronics', 'Electronic devices');

IF NOT EXISTS (SELECT 1 FROM [Instances].[Categories] WHERE Name = 'Clothing')
    INSERT INTO [Instances].[Categories] ([Name], [Description]) VALUES ('Clothing', 'Apparel');

IF NOT EXISTS (SELECT 1 FROM [Instances].[Categories] WHERE Name = 'Books')
    INSERT INTO [Instances].[Categories] ([Name], [Description]) VALUES ('Books', 'Books');

DECLARE @ElectronicsId INT = (SELECT TOP 1 InstanceId FROM [Instances].[Categories] WHERE Name = 'Electronics');
DECLARE @ClothingId INT = (SELECT TOP 1 InstanceId FROM [Instances].[Categories] WHERE Name = 'Clothing');
DECLARE @BooksId INT = (SELECT TOP 1 InstanceId FROM [Instances].[Categories] WHERE Name = 'Books');

IF @ElectronicsId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Instances].[CategoryAttributes] WHERE InstanceId = @ElectronicsId)
    INSERT INTO [Instances].[CategoryAttributes] ([InstanceId], [Key], [Value]) VALUES (@ElectronicsId, 'TaxCategory', 'Electronics');

IF @ClothingId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Instances].[CategoryAttributes] WHERE InstanceId = @ClothingId)
    INSERT INTO [Instances].[CategoryAttributes] ([InstanceId], [Key], [Value]) VALUES (@ClothingId, 'TaxCategory', 'Clothing');

IF NOT EXISTS (SELECT 1 FROM [Instances].[Products] WHERE Name = 'Wireless Mouse')
    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus]) 
    VALUES ('Wireless Mouse', 'Ergonomic wireless mouse', '[]', '["WM-001"]');

IF NOT EXISTS (SELECT 1 FROM [Instances].[Products] WHERE Name = 'USB-C Cable')
    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus]) 
    VALUES ('USB-C Cable', 'USB-C cable 6ft', '[]', '["UC-101"]');

IF NOT EXISTS (SELECT 1 FROM [Instances].[Products] WHERE Name = 'Bluetooth Headset')
    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus]) 
    VALUES ('Bluetooth Headset', 'Wireless headset', '[]', '["BH-201"]');

IF NOT EXISTS (SELECT 1 FROM [Instances].[Products] WHERE Name = 'Cotton T-Shirt')
    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus]) 
    VALUES ('Cotton T-Shirt', 'Cotton t-shirt', '[]', '["CT-BKXL"]');

IF NOT EXISTS (SELECT 1 FROM [Instances].[Products] WHERE Name = 'Running Shoes')
    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus]) 
    VALUES ('Running Shoes', 'Running shoes', '[]', '["RS-10"]');

IF NOT EXISTS (SELECT 1 FROM [Instances].[Products] WHERE Name = 'Programming Guide')
    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus]) 
    VALUES ('Programming Guide', 'Learn to code', '[]', '["PG-001"]');

DECLARE @MouseId INT = (SELECT TOP 1 InstanceId FROM [Instances].[Products] WHERE Name = 'Wireless Mouse');
DECLARE @CableId INT = (SELECT TOP 1 InstanceId FROM [Instances].[Products] WHERE Name = 'USB-C Cable');
DECLARE @HeadsetId INT = (SELECT TOP 1 InstanceId FROM [Instances].[Products] WHERE Name = 'Bluetooth Headset');
DECLARE @ShirtId INT = (SELECT TOP 1 InstanceId FROM [Instances].[Products] WHERE Name = 'Cotton T-Shirt');
DECLARE @ShoesId INT = (SELECT TOP 1 InstanceId FROM [Instances].[Products] WHERE Name = 'Running Shoes');
DECLARE @BookId INT = (SELECT TOP 1 InstanceId FROM [Instances].[Products] WHERE Name = 'Programming Guide');

IF @MouseId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Instances].[ProductAttributes] WHERE InstanceId = @MouseId)
BEGIN
    INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value]) VALUES (@MouseId, 'Color', 'Black');
    INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value]) VALUES (@MouseId, 'Wireless', 'Yes');
END

IF @CableId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Instances].[ProductAttributes] WHERE InstanceId = @CableId)
BEGIN
    INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value]) VALUES (@CableId, 'Color', 'White');
    INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value]) VALUES (@CableId, 'Length', '6ft');
END

IF @HeadsetId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Instances].[ProductAttributes] WHERE InstanceId = @HeadsetId)
BEGIN
    INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value]) VALUES (@HeadsetId, 'Color', 'Black');
    INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value]) VALUES (@HeadsetId, 'Wireless', 'Yes');
END

IF @MouseId IS NOT NULL AND @ElectronicsId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Instances].[ProductCategories] WHERE InstanceId = @MouseId AND CategoryInstanceId = @ElectronicsId)
    INSERT INTO [Instances].[ProductCategories] VALUES (@MouseId, @ElectronicsId);

IF @CableId IS NOT NULL AND @ElectronicsId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Instances].[ProductCategories] WHERE InstanceId = @CableId AND CategoryInstanceId = @ElectronicsId)
    INSERT INTO [Instances].[ProductCategories] VALUES (@CableId, @ElectronicsId);

IF @HeadsetId IS NOT NULL AND @ElectronicsId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Instances].[ProductCategories] WHERE InstanceId = @HeadsetId AND CategoryInstanceId = @ElectronicsId)
    INSERT INTO [Instances].[ProductCategories] VALUES (@HeadsetId, @ElectronicsId);

IF @ShirtId IS NOT NULL AND @ClothingId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Instances].[ProductCategories] WHERE InstanceId = @ShirtId AND CategoryInstanceId = @ClothingId)
    INSERT INTO [Instances].[ProductCategories] VALUES (@ShirtId, @ClothingId);

IF @ShoesId IS NOT NULL AND @ClothingId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Instances].[ProductCategories] WHERE InstanceId = @ShoesId AND CategoryInstanceId = @ClothingId)
    INSERT INTO [Instances].[ProductCategories] VALUES (@ShoesId, @ClothingId);

IF @BookId IS NOT NULL AND @BooksId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Instances].[ProductCategories] WHERE InstanceId = @BookId AND CategoryInstanceId = @BooksId)
    INSERT INTO [Instances].[ProductCategories] VALUES (@BookId, @BooksId);

IF @MouseId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Transactions].[InventoryTransactions] WHERE ProductInstanceId = @MouseId)
    INSERT INTO [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity], [TypeCategory]) VALUES (@MouseId, 50, 'InitialStock');

IF @CableId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Transactions].[InventoryTransactions] WHERE ProductInstanceId = @CableId)
    INSERT INTO [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity], [TypeCategory]) VALUES (@CableId, 100, 'InitialStock');

IF @HeadsetId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Transactions].[InventoryTransactions] WHERE ProductInstanceId = @HeadsetId)
    INSERT INTO [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity], [TypeCategory]) VALUES (@HeadsetId, 25, 'InitialStock');

IF @ShirtId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Transactions].[InventoryTransactions] WHERE ProductInstanceId = @ShirtId)
    INSERT INTO [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity], [TypeCategory]) VALUES (@ShirtId, 75, 'InitialStock');

IF @ShoesId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Transactions].[InventoryTransactions] WHERE ProductInstanceId = @ShoesId)
    INSERT INTO [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity], [TypeCategory]) VALUES (@ShoesId, 20, 'InitialStock');

IF @BookId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Transactions].[InventoryTransactions] WHERE ProductInstanceId = @BookId)
    INSERT INTO [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity], [TypeCategory]) VALUES (@BookId, 10, 'InitialStock');

PRINT 'Seed data completed!';
GO