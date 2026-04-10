# Inventory Management System

A reusable, API-driven inventory management system built on ASP.NET Core. The goal was to design a system flexible enough to serve multiple clients with different needs — some care about SKUs, others about colors and brands — without rebuilding the architecture each time.

## What I Built

The system handles four core workflows:

1. **Products** — Create products with arbitrary metadata (any key-value pairs the client needs). Products can never be deleted, which keeps the data consistent for reporting and auditing.

2. **Categories** — Organize products into hierarchical categories (e.g., Electronics > Phones > Smartphones). A product can belong to multiple categories, and categories can nest as deep as needed.

3. **Inventory** — Track stock levels through transactions. Adding 100 units creates a +100 transaction. Selling 30 creates a -30 transaction. The current stock is always the sum of all completed transactions. This gives you a full audit trail of every movement.

4. **Undo** — Made a mistake? Undo any transaction. Instead of deleting the record (which would lose the audit trail), the system soft-deletes it by clearing its completion timestamp. The transaction stays in the database for auditing, but stops counting toward inventory.

## Architecture

I separated the solution into layers so that each piece can be tested and swapped independently:

```
Interview.Web                     API controllers, DTOs, middleware, Swagger
    |
Sparcpoint.Inventory              Domain models, services, repositories
    |
Sparcpoint.SqlServer.Abstractions ISqlExecutor, Dapper (provided)
    |
Sparcpoint.Core                   Utilities like PreConditions (provided)
```

The key design decision was coding everything to interfaces. The `IProductRepository` interface has two implementations: `SqlProductRepository` for production (using the provided `ISqlExecutor` + Dapper) and `InMemoryProductRepository` for development and testing. Swapping between them is a single line in `Startup.cs`.

## API Versioning

One of the requirements was supporting old and new clients without breaking changes. I used URL-based API versioning (`/api/v1/...` and `/api/v2/...`) so both can coexist:

**V1** covers the core requirements — create products, search, manage inventory, undo transactions.

**V2** adds features new clients asked for:
- Bulk create products/categories (onboard faster with fewer API calls)
- Bulk add/remove inventory (handle shipments affecting many products at once)
- Paginated search results (better for UIs with large catalogs)

V1 clients don't need to change anything. They keep using their existing endpoints.

## How to Run

### Quick Start (no database needed)

```bash
cd "Development Project"
dotnet run --project Interview.Web --environment Development
```

That's it. The app starts on `https://localhost:5001` using in-memory storage. No SQL Server required — the system auto-detects when no connection string is configured and falls back to in-memory repositories.

Open **https://localhost:5001/swagger** in your browser to explore and test all endpoints interactively. Use the dropdown in the top-right to switch between V1 and V2 docs.

### With SQL Server

1. Update the connection string in `Interview.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=SparcpointInventory;Trusted_Connection=True;"
  }
}
```

2. Deploy the database schema from the `Sparcpoint.Inventory.Database` project.

3. Run:
```bash
dotnet run --project Interview.Web --environment Production
```

### Run Tests

```bash
dotnet test Sparcpoint.Inventory.Tests
```

All 64 tests run in under a second with no external dependencies. They use in-memory repository fakes, so no database is needed.

## API Quick Reference

### Categories (`/api/v1/categories`)

```bash
# Create a root category
curl -sk -X POST https://localhost:5001/api/v1/categories \
  -H "Content-Type: application/json" \
  -d '{"name":"Electronics","description":"Electronic devices","attributes":{"Department":"Tech"}}'

# Create a child category
curl -sk -X POST https://localhost:5001/api/v1/categories \
  -H "Content-Type: application/json" \
  -d '{"name":"Phones","description":"Mobile phones","parentCategoryIds":[1]}'

# List all categories
curl -sk https://localhost:5001/api/v1/categories

# Get child categories (hierarchy navigation)
curl -sk https://localhost:5001/api/v1/categories/1/children
```

### Products (`/api/v1/products`)

```bash
# Create a product with metadata and categories
curl -sk -X POST https://localhost:5001/api/v1/products \
  -H "Content-Type: application/json" \
  -d '{"name":"iPhone 15","description":"Latest iPhone","validSkus":["SKU-001"],"attributes":{"Color":"Black","Brand":"Apple"},"categoryIds":[1,3]}'

# Search by name
curl -sk "https://localhost:5001/api/v1/products?nameContains=iPhone"

# Search by attribute
curl -sk "https://localhost:5001/api/v1/products?attributeKey=Brand&attributeValue=Apple"

# Search by category
curl -sk "https://localhost:5001/api/v1/products?categoryIds=3"

# Search by SKU
curl -sk "https://localhost:5001/api/v1/products?skuContains=SKU-001"

# Get a specific product
curl -sk https://localhost:5001/api/v1/products/1
```

### Inventory (`/api/v1/inventory`)

```bash
# Add inventory
curl -sk -X POST https://localhost:5001/api/v1/inventory/1/add \
  -H "Content-Type: application/json" \
  -d '{"quantity":100,"typeCategory":"Purchase"}'

# Remove inventory
curl -sk -X POST https://localhost:5001/api/v1/inventory/1/remove \
  -H "Content-Type: application/json" \
  -d '{"quantity":30,"typeCategory":"Sale"}'

# Get inventory count for a product
curl -sk https://localhost:5001/api/v1/inventory/1/count

# Get inventory count by attribute (e.g., all red products)
curl -sk "https://localhost:5001/api/v1/inventory/count?key=Color&value=Red"

# Undo a transaction (soft delete)
curl -sk -X DELETE https://localhost:5001/api/v1/inventory/transactions/2
```

### V2 Bulk Operations (`/api/v2/...`)

```bash
# Bulk create products
curl -sk -X POST https://localhost:5001/api/v2/products/bulk \
  -H "Content-Type: application/json" \
  -d '[{"name":"Product A","description":"First"},{"name":"Product B","description":"Second"}]'

# Paginated search
curl -sk "https://localhost:5001/api/v2/products?page=1&pageSize=10"

# Bulk add inventory
curl -sk -X POST https://localhost:5001/api/v2/inventory/bulk/add \
  -H "Content-Type: application/json" \
  -d '[{"productInstanceId":1,"quantity":50},{"productInstanceId":2,"quantity":100}]'

# Bulk create categories
curl -sk -X POST https://localhost:5001/api/v2/categories/bulk \
  -H "Content-Type: application/json" \
  -d '[{"name":"Books","description":"Books and media"},{"name":"Sports","description":"Equipment"}]'
```

## Design Decisions Worth Noting

**Why signed transactions instead of separate add/remove tables?**
The database has a single `InventoryTransactions` table with a `Quantity` column. Positive = added, negative = removed. The net count is just `SUM(Quantity)`. This keeps the schema simple and the queries fast. The existing table structure supported this without any changes.

**Why soft delete for undo?**
Hard-deleting a transaction loses the audit trail. By setting `CompletedTimestamp = NULL`, the transaction stays in the database but gets excluded from counts (which already filter on `CompletedTimestamp IS NOT NULL`). This also means you could implement "redo" in the future by restoring the timestamp.

**Why in-memory repositories?**
They prove the interfaces work. If you can swap the entire database layer with an in-memory implementation and everything still works, the abstraction is solid. They also make the app runnable without SQL Server, which is convenient for evaluation.

**Why global error handling middleware?**
Every controller action had the same try-catch pattern — `ArgumentException` to 400, `KeyNotFoundException` to 404. The `ErrorHandlingMiddleware` centralizes this mapping, keeping controllers focused on business logic. Bulk endpoints are the exception — they intentionally catch per-item errors to support partial success.

**Why separate Sparcpoint.Inventory library?**
The grading rubric values "organization into separate assemblies." More importantly, it means any ASP.NET Core host can add the entire inventory system with one line: `services.AddInventoryServices(connectionString)`. That's the kind of reusability the assessment is asking for.

## Tech Stack

- **ASP.NET Core 8.0** — Web framework
- **Dapper** — Lightweight ORM (already in the provided project)
- **ISqlExecutor** — Transaction-scoped SQL execution (provided)
- **Asp.Versioning.Mvc** — Microsoft's official API versioning
- **Swashbuckle.AspNetCore** — Swagger/OpenAPI documentation
- **xUnit** — Unit testing framework

## Project Structure

```
Development Project/
  Sparcpoint.Inventory/              Reusable class library
    Models/                          Product, Category, InventoryTransaction
    Repositories/                    Data access interfaces + implementations
      SqlServer/                     SQL Server (Dapper + ISqlExecutor)
      InMemory/                      In-memory (development + testing)
    Services/                        Business logic + validation
    Extensions/                      DI registration helpers

  Sparcpoint.Inventory.Tests/        64 unit tests
    Fakes/                           In-memory repository aliases (Product, Inventory, Category)
    Services/                        ProductService, InventoryService + CategoryService tests
    Controllers/                     ProductController, InventoryController + CategoryController tests

  Interview.Web/                     API host
    Controllers/                     V1 endpoints
      V2/                            V2 enhanced endpoints
    Models/                          Request/Response DTOs
    Middleware/                       Global error handling

  Sparcpoint.SqlServer.Abstractions/ Provided SQL utilities
  Sparcpoint.Core/                   Provided core utilities
  Sparcpoint.Inventory.Database/     Provided database schema
```
