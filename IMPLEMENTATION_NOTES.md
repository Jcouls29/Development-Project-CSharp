# Implementation Notes

This file is meant to make the project easier to explain, maintain, and extend.

## What Was Built

The assignment started from a mostly empty ASP.NET Core API and a provided SQL schema. The implementation adds a working inventory API with these flows:

- create categories, including parent/child category relationships
- create products with free-form metadata and category assignments
- search products by name, description, SKU, attributes, and categories
- add and remove inventory in batches
- delete an inventory transaction as a simple undo operation
- retrieve inventory counts by product or filter set

## Request Flow

Most requests follow the same path:

1. A controller receives the HTTP request.
2. `InventoryManagementService` validates and normalizes the request.
3. `SqlInventoryRepository` translates that normalized request into SQL.
4. `ISqlExecutor` runs the SQL inside the provided abstraction and transaction boundary.
5. The repository maps rows back into response contracts.

That split is useful because it keeps input cleanup out of the controller and keeps SQL rules out of the service.

## Main Files

- `Development Project/Interview.Web/Controllers/ProductController.cs`
  - create products
  - get all products
  - get a single product
  - search products
- `Development Project/Interview.Web/Controllers/CategoryController.cs`
  - create categories
  - list categories
- `Development Project/Interview.Web/Controllers/InventoryController.cs`
  - add inventory
  - remove inventory
  - delete a transaction
  - query counts
- `Development Project/Interview.Web/Services/InventoryManagementService.cs`
  - trims strings
  - rejects bad input
  - merges duplicate inventory items
  - deduplicates category ids and SKU lists
- `Development Project/Interview.Web/Data/SqlInventoryRepository.cs`
  - performs all inserts, reads, searches, and inventory calculations

## How The Schema Is Being Used

The solution stays inside the provided schema instead of introducing new tables.

- `Instances.Products`
  - core product row
  - `ProductImageUris` is stored as JSON text
  - `ValidSkus` is stored as JSON text
- `Instances.ProductAttributes`
  - one row per product attribute key/value
- `Instances.Categories`
  - core category row
- `Instances.CategoryAttributes`
  - one row per category attribute key/value
- `Instances.ProductCategories`
  - links products to categories
- `Instances.CategoryCategories`
  - links a child category to a parent category
- `Transactions.InventoryTransactions`
  - stores inventory adds and removes as signed quantities

## Category Hierarchy Interpretation

This is the main part that can feel confusing at first.

The current implementation treats `CategoryCategories` like this:

- `InstanceId` = child category
- `CategoryInstanceId` = parent category

So if category `5` is a child of category `2`, the link row is:

- `InstanceId = 5`
- `CategoryInstanceId = 2`

When a search or count request sets `IncludeDescendantCategories = true`, the repository uses a recursive query to start from the requested parent category ids and walk downward to include children.

That means a search for `Tools` can also match `Hammers`, `Screwdrivers`, and any deeper descendants linked under `Tools`.

## Validation And Normalization Rules

Before SQL runs, the service layer cleans up requests:

- trims surrounding spaces from names, descriptions, SKUs, and attribute values
- rejects missing required fields
- rejects non-positive ids and quantities
- removes duplicate category ids
- removes duplicate SKUs ignoring case
- merges duplicate inventory items by `ProductId`
- defaults inventory `TypeCategory` values to:
  - `manual-add` for add requests
  - `manual-remove` for remove requests

This is why the API tends to behave consistently even when the request body is a bit messy.

## Current Automated Tests

The test project is intentionally small and focused on the service layer. Right now it covers:

- trimming and deduplicating product input
- aggregating duplicate inventory lines
- rejecting invalid inventory quantities
- rejecting duplicate attribute keys ignoring case
- normalizing search filters
- normalizing inventory count filters
- normalizing category creation input

These tests do not hit SQL Server. They are fast checks around the behavior that is easiest to break while refactoring.

## Good Manual Test Scenarios

If you want to keep exploring the project, this order gives good coverage.

### 1. Build a small category tree

Create a top-level category:

```json
{
  "name": "Tools",
  "description": "Top level tools category",
  "attributes": {
    "department": "Hardware"
  },
  "parentCategoryIds": []
}
```

Then a child category:

```json
{
  "name": "Hammers",
  "description": "Hammer family",
  "attributes": {
    "aisle": "A1"
  },
  "parentCategoryIds": [1]
}
```

### 2. Add a few products under different categories

Example product in `Hammers`:

```json
{
  "name": "Blue Hammer",
  "description": "16oz claw hammer",
  "productImageUris": [],
  "validSkus": ["HAM-001"],
  "attributes": {
    "brand": "Acme",
    "color": "Blue"
  },
  "categoryIds": [2]
}
```

Add a second product with a different brand or category so searches have something to compare against.

### 3. Test category hierarchy searches

Search by the parent category id and keep `IncludeDescendantCategories = true`. Products assigned to child categories should still match.

### 4. Test direct-only category searches

Search again with the same category id but set `IncludeDescendantCategories = false`. Only directly assigned products should match.

### 5. Test batch inventory

Send one add request with multiple items:

```json
{
  "typeCategory": "initial-load",
  "items": [
    { "productId": 1, "quantity": 10 },
    { "productId": 2, "quantity": 5 }
  ]
}
```

Then remove some stock and verify the total changes.

### 6. Test undo

Delete the removal transaction and verify the count returns to the earlier value.

## Tradeoffs And Next Improvements

The current solution is a good interview-sized MVP, but there are natural next steps:

- add integration tests that hit a temporary SQL database
- move from JSON-in-text SKU searching to a more structured storage model if the schema ever evolves
- add paging to product searches if the data set grows
- switch the provided SQL client usage from obsolete APIs to `Microsoft.Data.SqlClient`
- add a small Postman collection or HTTP file for even faster manual testing

## How To Explain It In A Review

If someone asks what you built, a clean answer is:

"I kept the provided schema and SQL executor, added an application service for validation and normalization, implemented the repository SQL for products, categories, and inventory transactions, then added a small test project plus setup/demo scripts so the solution can be run and verified locally."
