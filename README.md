# Development Project CSharp

This repository contains a small ASP.NET Core inventory API built to satisfy the provided interview exercise. The implementation focuses on the highest-value API flows from the spec:

- create categories and category hierarchies
- create products with metadata and category assignments
- search products by general details, attributes, and categories
- add and remove inventory in batches
- delete individual inventory transactions as a simple undo mechanism
- retrieve inventory counts by product or filter set

## Projects

- `Development Project/Interview.Web`: API surface and application logic
- `Development Project/Sparcpoint.SqlServer.Abstractions`: provided SQL execution abstraction reused by the API
- `Development Project/Sparcpoint.Inventory.Database`: provided schema files
- `Development Project/Interview.Web.Tests`: small automated tests around request validation and normalization

## Understand The Solution

If you are coming back to the project after some time away, start with:

- `IMPLEMENTATION_NOTES.md`: architecture, request flow, schema mapping, and tradeoffs
- `Development Project/Interview.Web/Controllers`: API endpoints
- `Development Project/Interview.Web/Services/InventoryManagementService.cs`: validation and normalization
- `Development Project/Interview.Web/Data/SqlInventoryRepository.cs`: SQL behavior against the provided schema

## Local Setup

1. Reset and create the local `inventory` database:

   ```powershell
   powershell -ExecutionPolicy Bypass -File .\Setup-InventoryDatabase.ps1 -Reset
   ```

2. Restore packages:

   ```powershell
   dotnet restore ".\Development Project\Development Project.sln" --configfile ".\NuGet.Config"
   ```

3. Start the API:

   ```powershell
   dotnet run --project ".\Development Project\Interview.Web\Interview.Web.csproj"
   ```

4. Open the default endpoint:

   ```text
   http://localhost:5000/api/v1/products
   ```

## Run Tests

```powershell
dotnet restore ".\Development Project\Interview.Web.Tests\Interview.Web.Tests.csproj" --configfile ".\NuGet.Config"
dotnet test ".\Development Project\Interview.Web.Tests\Interview.Web.Tests.csproj" --no-restore
```

## End-To-End Demo

After resetting the database and starting the API, run:

```powershell
powershell -ExecutionPolicy Bypass -File .\Demo-InventoryApi.ps1
```

That script will:

- create a category
- create a product
- add inventory
- search by attribute
- remove inventory
- delete the removal transaction to simulate undo
- print the resulting counts at each step

## Example API Calls

Create a category:

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5000/api/v1/categories" -ContentType "application/json" -Body '{"name":"Hardware","description":"General hardware items","attributes":{"department":"Tools"},"parentCategoryIds":[]}'
```

Create a product:

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5000/api/v1/products" -ContentType "application/json" -Body '{"name":"Blue Hammer","description":"16oz claw hammer","productImageUris":[],"validSkus":["HAM-001"],"attributes":{"color":"Blue","brand":"Acme"},"categoryIds":[1]}'
```

Add inventory:

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5000/api/v1/inventory/add" -ContentType "application/json" -Body '{"typeCategory":"initial-load","items":[{"productId":1,"quantity":25}]}'
```

Query inventory:

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5000/api/v1/inventory/counts/query" -ContentType "application/json" -Body '{"productId":1}'
```

Search by attribute:

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5000/api/v1/products/search" -ContentType "application/json" -Body '{"attributes":{"brand":"Acme"}}'
```

## Notes

- `ProductImageUris` and `ValidSkus` are stored as JSON strings inside the provided `Products` table rather than introducing new tables.
- The project keeps the provided SQL schema and uses the supplied `ISqlExecutor` abstraction rather than introducing a new persistence stack.
- In development, the API runs on plain HTTP at `http://localhost:5000` to avoid local certificate setup friction.
