# Sparcpoint.Inventory.IntegrationTests

Integration tests for the inventory layer. Each test spins up a fresh isolated database in LocalDB, exercises the real `ISqlExecutor` against real SQL, then tears the database down — no mocks.

## Prerequisites

You already have these if you can run the main `Interview.Web` project:

- .NET 8 SDK
- SQL Server LocalDB (ships with Visual Studio's *.NET desktop development* and *Data storage and processing* workloads)

No separate database deployment is needed for tests — the fixture creates a unique database on each run (`SparcpointInventory_Test_<guid>`) and drops it when finished.

## Running the tests

### From the command line

From the solution root (`Development Project/`):

```
dotnet test "Sparcpoint.Inventory.IntegrationTests/Sparcpoint.Inventory.IntegrationTests.csproj"
```

Or to run the whole solution's tests:

```
dotnet test
```

### From Visual Studio

1. Open `Development Project.sln`
2. Open **Test → Test Explorer**
3. Click **Run All Tests**

Tests appear under `Sparcpoint.Inventory.IntegrationTests` grouped by class:
- `ProductRepositoryTests` — create, search by name / category / attribute / combination, edge cases
- `CategoryRepositoryTests` — create, hierarchies, retrieval
- `InventoryRepositoryTests` — bulk add/remove, undo via delete, count by id and/or attribute filter, negative quantities

## What gets tested

| Requirement | Covered by |
|---|---|
| #1 Products never deleted | No DELETE in `IProductRepository` (compile-time guarantee) |
| #2 Arbitrary metadata + categories | `ProductRepositoryTests.Create_Then_GetById_RoundTripsAllFields` |
| #3 Searchable by metadata + categories | `Search_ByAttribute_*`, `Search_ByCategory_*`, `Search_ByCategoryAndAttribute_*` |
| #4 Bulk inventory adjustments | `InventoryRepositoryTests.BulkTransactions_ApplyAtomically_AcrossMultipleProducts` |
| #5 Count by product or metadata subset | `GetCount_ByAttributeFilter_*`, `GetCount_ByProductIdAndAttribute_*` |
| #6 Individual transactions removable | `DeleteTransaction_UndoesItsEffectOnCount` |
| #7 API-driven | The full stack from `ISqlExecutor` through the repositories is exercised |

Edge cases verified:
- Missing record retrieval (returns null)
- Deleting a non-existent transaction (throws `KeyNotFoundException` → controller maps to 404)
- VARCHAR overrun (long names truncated, no `SqlException`)
- Null/empty descriptions
- Negative quantities (inventory removal)
- Empty product with no transactions (count = 0)

## Test isolation

Tests within the suite share a single LocalDB database (created once per run, dropped at end) but each test resets the data via `TestDatabaseFixture.ResetAsync()` in its `InitializeAsync`. Test order and parallelism cannot leak state between tests.
