# Inventory Management System

A reusable, API-driven inventory management system built on ASP.NET Core 8. Designed to handle multiple clients with different product metadata needs without rebuilding the core logic each time.

## What it does

- Create products with arbitrary key/value metadata and category assignments
- Search products by name, category, or any metadata attribute
- Add and remove inventory per product (or in bulk)
- Query inventory counts by product or filtered subset
- Undo individual transactions by deleting them

Products are never deleted — once added, they stay in the system. Inventory is tracked as a ledger of transactions, so the on-hand count is always the net SUM.

## Prerequisites

- .NET 8 SDK
- SQL Server LocalDB (ships with Visual Studio) or any SQL Server instance
- The `SparcpointInventory` database with the schema applied

## Database setup

Run the SQL scripts in the project against your SQL Server instance before starting the API. The schema creates tables under two schemas: `Instances` (products, categories, attributes) and `Transactions` (inventory ledger).

Default connection string targets `(localdb)\MSSQLLocalDB`. To use a different instance, update `ConnectionStrings:InventoryDatabase` in `Interview.Web/appsettings.json`.

## Running the API

```bash
cd "Interview.Web"
dotnet run
```

Swagger UI is available at `https://localhost:{port}/swagger` in all environments. Use it to explore and test every endpoint with example payloads.

## Project structure

```
Sparcpoint.Inventory.Abstractions/   Interfaces, models, request/filter types
Sparcpoint.Inventory.SqlServer/      SQL Server implementations (Dapper)
Sparcpoint.SqlServer.Abstractions/   ISqlExecutor, SqlServerQueryProvider (existing)
Interview.Web/                       ASP.NET Core API, controllers, filters, Startup
Sparcpoint.Inventory.Tests/          Unit tests (Moq) + integration tests (LocalDB)
```

The Abstractions project defines the contracts. The SqlServer project implements them. The web project wires everything together and exposes it over HTTP. Any new host (worker service, CLI tool) just references SqlServer and calls `services.AddSqlInventoryServices(connectionString)`.

## API overview

### Products

| Method | Route | Description |
|--------|-------|-------------|
| `POST` | `/api/v1/products` | Create a product with metadata and categories |
| `GET` | `/api/v1/products/search` | Search by name, categoryIds, attributeKey/Value, page/pageSize |

### Categories

| Method | Route | Description |
|--------|-------|-------------|
| `POST` | `/api/v1/categories` | Create a category with optional parent IDs |
| `GET` | `/api/v1/categories` | List all categories |

### Inventory

| Method | Route | Description |
|--------|-------|-------------|
| `POST` | `/api/v1/inventory/{productId}/add` | Add stock — returns transactionId |
| `POST` | `/api/v1/inventory/{productId}/remove` | Remove stock — returns transactionId |
| `POST` | `/api/v1/inventory/batch/add` | Add stock for multiple products (atomic) |
| `POST` | `/api/v1/inventory/batch/remove` | Remove stock for multiple products (atomic) |
| `DELETE` | `/api/v1/inventory/transactions/{id}` | Delete a transaction (undo its effect) |
| `GET` | `/api/v1/inventory/{productId}/count` | Net inventory count for a product |
| `GET` | `/api/v1/inventory/count` | Counts filtered by name, category, or attribute |

Full request/response examples are in the Swagger UI.

## Example: create a product

```json
POST /api/v1/products
{
  "name": "Widget Pro",
  "description": "Industrial-grade widget",
  "validSkus": "WGT-PRO-BLK,WGT-PRO-WHT",
  "attributes": {
    "Brand": "ACME",
    "Color": "Black",
    "SKU": "WGT-PRO-BLK"
  },
  "categoryIds": [1]
}
```

Returns `201` with the new product ID.

## Example: add inventory

```json
POST /api/v1/inventory/1/add
{
  "quantity": 50,
  "typeCategory": "PURCHASE"
}
```

Returns `201` with the transactionId. To undo: `DELETE /api/v1/inventory/transactions/{transactionId}`.

## Key design decisions

**Dapper over Entity Framework** — the EAV attribute schema and dynamic WHERE building work better with explicit SQL. Dapper sits cleanly on top of the existing `ISqlExecutor` abstraction without replacing it.

**Attribute search uses EXISTS subqueries** — joining on EAV tables causes row multiplication before you can deduplicate. One correlated EXISTS block per attribute avoids that entirely and has a more predictable query plan.

**Inventory as a ledger** — there's no "current stock" column. Adds insert positive quantities, removes insert negative. The count is always `SUM(Quantity) WHERE CompletedTimestamp IS NOT NULL`. Undo = delete the row. Simple, auditable, no compensating transactions needed.

**FK pre-validation** — referenced IDs (product, category) are validated with an EXISTS check inside the same transaction before each INSERT. This converts SQL error 547 (FK violation) from an unhandled 500 into a clean 400 with a readable message. The global `ApiExceptionFilter` also catches any 547 that slips through as a TOCTOU fallback.

**Single extension method for DI** — `services.AddSqlInventoryServices(connectionString)` wires all repositories. Adding a new one means touching one file, not every host project.

## Running the tests

### Unit tests

```bash
dotnet test --filter "Category!=Integration"
```

These use Moq to mock `ISqlExecutor` — no database needed, runs in CI without any setup.

### Integration tests

Require the `SparcpointInventory` database to be up with the schema applied.

```bash
# optional: point at a different SQL Server
$env:INVENTORY_TEST_DB = "Server=myserver;Database=SparcpointInventory;Trusted_Connection=True;"

dotnet test --filter "Category=Integration"
```

Integration tests use `TransactionScope` without `Complete()` — everything auto-rolls back after each test, so no cleanup scripts are needed and the database stays clean between runs.

## Error handling

| HTTP | Cause |
|------|-------|
| `400` | Invalid input (missing required fields, bad values, referenced ID doesn't exist) |
| `404` | Transaction not found on delete |
| `500` | Unexpected — shouldn't happen in normal usage |

All error responses follow `{ "error": "message" }`.
