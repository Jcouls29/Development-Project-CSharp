# Sparcpoint Inventory

A reusable inventory management API built on the provided ASP.NET Core 8 / SQL
Server scaffolding. Products carry arbitrary metadata and categories;
inventory is tracked via an append-only transaction log with individual-row
undo support.

## Running

```bash
cd "Development Project/Interview.Web"
dotnet run --urls=http://localhost:5080
```

Interactive API docs: <http://localhost:5080/swagger>

The app defaults to **in-memory repositories** (`appsettings.json` →
`Inventory:UseInMemoryRepositories: true`) so no database is required to
exercise the endpoints. Flip the flag and provide a `ConnectionString` to run
against SQL Server.

## Tests

```bash
dotnet test "Development Project/Sparcpoint.Inventory.Tests"
```

19 xUnit tests covering service-layer behavior using in-memory repositories.

## Solution layout

```
Development Project/
├── Interview.Web/                     ASP.NET Core 8 API host
├── Sparcpoint.Inventory/              [new] Domain library
│   ├── Abstractions/                  Service + repository interfaces
│   ├── Models/                        Domain types
│   ├── Services/                      Business logic + validation
│   ├── Repositories/
│   │   ├── InMemory/                  Dev + test fakes
│   │   └── Sql/                       Dapper + ISqlExecutor
│   └── Extensions/                    Options + DI registration
├── Sparcpoint.Inventory.Tests/        [new] xUnit tests
├── Sparcpoint.Core/                   Given (unchanged)
├── Sparcpoint.SqlServer.Abstractions/ Given (unchanged)
└── Sparcpoint.Inventory.Database/     Given SSDT schema (unchanged)
```

## API surface (all under `/api/v1/`)

| Method | Route | Purpose |
|---|---|---|
| POST | `/products` | Create product with attributes + categories |
| GET | `/products/{id}` | Fetch product + metadata |
| POST | `/products/search` | Filter by name/description/category/attributes/SKU |
| POST | `/categories` | Create category (optional parents) |
| GET | `/categories` | List all |
| GET | `/categories/{id}` | Fetch one |
| PUT | `/categories/{id}` | Update |
| DELETE | `/categories/{id}` | Remove |
| GET | `/categories/{id}/descendants` | Walk hierarchy |
| POST | `/inventory/add` | Record a positive transaction |
| POST | `/inventory/remove` | Record a negative transaction |
| POST | `/inventory/bulk` | Record many transactions atomically |
| DELETE | `/inventory/transactions/{id}` | Undo a single transaction |
| GET | `/inventory/count/{productId}` | Summed inventory for a product |
| GET | `/inventory/count/by-attribute?key=&value=` | Sum grouped by matching products |

## Requirements coverage (spec §3)

| # | Requirement | Status |
|---|---|---|
| 1 | Products added, never deleted | Yes — no delete endpoint |
| 2 | Arbitrary metadata + categories | Yes — `ProductAttributes`, `ProductCategories` |
| 3 | Searchable by metadata + categories | Yes — `POST /products/search` |
| 4 | Add / remove inventory, single or bulk | Yes — `/add`, `/remove`, `/bulk` |
| 5 | Retrieve counts per product or by metadata | Yes — `/count/{id}`, `/count/by-attribute` |
| 6 | Individual transactions removable | Yes — `DELETE /inventory/transactions/{id}` |
| 7 | API-driven | Yes — ASP.NET Core + Swagger |

## Architectural highlights

- **Three-layer separation** — controllers → services → repositories. Services
  and controllers never touch `ISqlExecutor` or Dapper directly.
- **Two repository implementations** behind the same interfaces — SQL
  (`ISqlExecutor` + Dapper) and in-memory (`ConcurrentDictionary`). Swappable
  via DI.
- **Options-driven configuration** — `InventoryOptions` binds from
  `appsettings.json`; a single `AddInventory()` extension wires the domain.
- **Route versioning** (`/api/v1/...`) to allow breaking changes for future
  customers without impacting existing ones.
- **Validation at the service boundary** — `ArgumentException` is mapped to
  400 Bad Request (`ProblemDetails`) by a global exception filter.
- **`EVAL:` comments** mark non-obvious decisions (negative-quantity storage
  for removes, dynamic `JOIN` search, recursive CTE for category hierarchy).

## Third-party libraries

| Package | Reason |
|---|---|
| Dapper | Spec-recommended; composes with the provided `ISqlExecutor`. |
| Swashbuckle.AspNetCore | OpenAPI / Swagger UI for API discovery. |
| xUnit | De-facto .NET test framework. |

## Known gaps (deliberate for a 2-hour scope)

- SQL repositories are implemented but not integration-tested — would need a
  LocalDB deployment of the schema. In-memory repos test service behavior.
- No authentication / authorization.
- The existing `Sparcpoint.Inventory.Database.sqlproj` requires Visual Studio
  SSDT MSBuild targets to build via CLI — unrelated to this work.
