# Interview.Web

## Objective

`Interview.Web` is an ASP.NET Core Web API for managing products, categories, product metadata, and inventory activity. Products can be created, listed, and searched by name, category, or metadata, while inventory is tracked through transaction records such as purchases, sales, and adjustments.

The application uses Entity Framework Core with SQL Server for persistence and separates controller logic from business/data access logic through service classes and dependency injection. Unit tests in `Interview.Web.Tests` validate the service layer using EF Core InMemory.

## EF Core

This project uses Entity Framework Core with SQL Server. The database connection is configured in `appsettings.json` under `ConnectionStrings:DefaultConnection`.

### Initial setup

From the repository root, move into the web project:

```powershell
cd "Development Project/Interview.Web"
```

If the EF CLI is not already installed, install or update it:

```powershell
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

Restore packages and apply the existing migrations:

```powershell
dotnet restore
dotnet ef database update
```

This creates or updates the local database using the migrations in the `Migrations` folder.

### Common EF commands

Create a new migration after changing EF models or `ApplicationDbContext`:

```powershell
dotnet ef migrations add MigrationNameHere
```

Apply pending migrations to the database:

```powershell
dotnet ef database update
```

Run the web API:

```powershell
dotnet run
```

### Start fresh locally

To clear everything and rebuild the local database from migrations:

```powershell
dotnet ef database drop --force
dotnet ef database update
```

If the database is in use, stop the running API first and rerun the commands.

To clear data but keep the database and schema, delete rows in dependency order:

```sql
DELETE FROM InventoryTransactions;
DELETE FROM ProductCategories;
DELETE FROM Products;
DELETE FROM Categories;
```

## Basic Usage

These examples assume the API is running locally at `https://localhost:5001`.

### Add a product

Send a `POST` request to:

```http
POST https://localhost:5001/api/v1/products
```

Example JSON body:

```json
{
  "name": "Test Product",
  "description": "A sample product used for local testing.",
  "price": 19.99,
  "categories": [
    "test"
  ],
  "metadata": {
    "color": "blue",
    "size": "medium"
  }
}
```

This creates the product, creates any missing categories, links the product to those categories, and returns a basic response with the created product ID and name.

### Get all products

Send a `GET` request to:

```http
GET https://localhost:5001/api/v1/products
```

This returns all products with their basic fields, metadata, and category names.

### Search products by category

Send a `GET` request to:

```http
GET https://localhost:5001/api/v1/products/search?category=test
```

This returns products assigned to the `test` category. The search endpoint also supports other query string filters, but this example focuses on category-based filtering.

### Create an inventory transaction

Send a `POST` request to:

```http
POST https://localhost:5001/api/v1/inventory-transactions
```

Example JSON body:

```json
{
  "productId": "2cf8ad80-2e7f-4f29-a362-19ea656581da",
  "quantity": 15.5,
  "type": "Purchase"
}
```

This creates an inventory transaction for an existing product. The `type` value should match one of the supported inventory transaction types, such as `Purchase`, `Sale`, or `Adjustment`. A `Purchase` or `Adjustment` increases available inventory, while a `Sale` is subtracted from available inventory.

### Get inventory transactions for a product

Send a `GET` request to:

```http
GET https://localhost:5001/api/v1/inventory-transactions/2cf8ad80-2e7f-4f29-a362-19ea656581da
```

This returns all inventory transaction records for the provided product ID, ordered with the newest transactions first.

### Get current inventory for a product

Send a `GET` request to:

```http
GET https://localhost:5001/api/v1/inventory/2cf8ad80-2e7f-4f29-a362-19ea656581da
```

This returns the computed inventory summary for the product:

```json
{
  "productId": "2cf8ad80-2e7f-4f29-a362-19ea656581da",
  "onHandQuantity": 15
}
```

The `onHandQuantity` value is calculated from inventory transactions as adjustments plus purchases minus sales.

## Unit Test Overview

The `Interview.Web.Tests` project uses xUnit and EF Core InMemory to validate service-layer behavior without requiring SQL Server.

### InventoryServiceTests

| Test name | Expected outcome |
| --- | --- |
| `GetInventoryByProductId_ProductNotFound_ReturnsNull` | Returns `null` when no product exists for the requested product ID. |
| `GetInventoryByProductId_NoTransactions_ReturnsZero` | Returns an inventory result with `OnHandQuantity` equal to `0` for a valid product with no transactions. |
| `GetInventoryByProductId_CalculatesAdjustmentPurchaseMinusSale` | Calculates on-hand inventory as adjustments plus purchases minus sales. |
| `GetInventoryByProductId_SalesOnly_SubtractsFromZero` | Returns a negative on-hand quantity when only sale transactions exist. |
| `GetInventoryByProductId_IgnoresOtherProducts` | Only includes transactions for the requested product ID. |

### InventoryTransactionServiceTests

| Test name | Expected outcome |
| --- | --- |
| `CreateInventoryTransaction_ProductNotFound_ReturnsNull` | Returns `null` and does not create a transaction when the product ID does not exist. |
| `CreateInventoryTransaction_ValidProduct_PersistsTransaction` | Creates and persists a transaction for an existing product, then returns the mapped response DTO. |
| `GetInventoryTransactionsByProductId_NoTransactions_ReturnsEmpty` | Returns an empty collection when no transactions exist for the requested product ID. |
| `GetInventoryTransactionsByProductId_ReturnsOnlyMatchingProductTransactions` | Returns only transactions matching the requested product ID. |
| `GetInventoryTransactionsByProductId_ReturnsNewestTransactionsFirst` | Returns matching transactions ordered by newest `CreatedAt` first. |

### ProductServiceTests

| Test name | Expected outcome |
| --- | --- |
| `GetAllProducts_EmptyDatabase_ReturnsEmptyList` | Returns an empty collection when no products exist. |
| `GetAllProducts_ReturnsMappedProductFields` | Maps product fields and metadata into the response DTO. |
| `GetAllProducts_IncludesCategoryNames` | Includes category names for products with categories. |
| `GetAllProducts_ReturnsMultipleProducts` | Returns all products stored in the database. |
| `GetAllProducts_ProductWithMultipleCategories_ReturnsAllCategoryNames` | Returns all category names for a product with multiple categories. |
| `AddProduct_PersistsProductWithMappedFields` | Persists a new product and returns the mapped response DTO. |
| `AddProduct_WithCategories_ReturnsCategoryNames` | Creates new categories, links them to the product, and returns their names. |
| `AddProduct_ReusesExistingCategory` | Reuses an existing category instead of creating duplicates. |
| `AddProduct_NullMetadata_UsesEmptyDictionary` | Uses an empty metadata dictionary when the request metadata is `null`. |
| `Search_NoFilters_ReturnsAllProducts` | Returns all products when no search filters are provided. |
| `Search_ByQuery_MatchesName` | Returns products whose name contains the query text. |
| `Search_ByQuery_MatchesDescription` | Returns products whose description contains the query text. |
| `Search_ByCategory_FiltersCorrectly` | Returns products assigned to the requested category. |
| `Search_ByMetadataKeyValue_FiltersCorrectly` | Returns products with matching metadata key and value. |
| `Search_CombinedFilters_ReturnsMatchingProduct` | Applies query, category, and metadata filters together. |
| `Search_NoMatches_ReturnsEmpty` | Returns an empty collection when no products match the filters. |
