# Load Tests

Load tests for the Inventory Management API using [k6](https://k6.io).

## Prerequisites

- The Inventory API must be running locally before executing any load test
- A SQL Server instance with the inventory database must be accessible

## Tools Required

| Tool | Version | Purpose |
|---|---|---|
| [k6](https://k6.io) | Latest | Load test runner |

## Install k6

**Windows (winget)**
```bash
winget install k6 --source winget
```

**Windows (Chocolatey)**
```bash
choco install k6
```

**Windows (manual)**
Download the installer from https://github.com/grafana/k6/releases and run the `.msi`.

**macOS (Homebrew)**
```bash
brew install k6
```

Verify the install:
```bash
k6 version
```

## Start the API

The API must be running before executing tests. From the solution root:
```bash
dotnet run --project "Development Project/Interview.Web"
```

Or set your connection string via environment variable first:
```bash
# Windows (PowerShell)
$env:INVENTORY_DB = "Server=YOUR_SERVER;Database=inventory;Integrated Security=True;TrustServerCertificate=True;"
dotnet run --project "Development Project/Interview.Web"
```

## Configuration

Before running, update the constants at the top of `basic-load-test.js` to match IDs that exist in your database:

```js
const BASE_URL     = 'https://localhost:5001'; // API base URL
const PRODUCT_ID   = 1;                        // must exist in your DB
const CATEGORY_ID  = 1;                        // must exist in your DB
```

## Run the Load Test

From the `LoadTests/` directory:
```bash
k6 run basic-load-test.js
```

## What the Test Does

`basic-load-test.js` simulates a realistic mix of concurrent users:

| Scenario | Share | Endpoint |
|---|---|---|
| Add inventory | 40% | `POST /api/v1/inventory/add` |
| Get inventory count | 30% | `GET /api/v1/inventory/count/{id}` |
| Search by attribute | 20% | `POST /api/v1/products/search` |
| Search by category | 10% | `POST /api/v1/products/search` |

**Load stages:**

| Stage | Duration | Virtual Users |
|---|---|---|
| Ramp up | 15s | 0 → 10 |
| Ramp up | 30s | 10 → 50 |
| Ramp up | 30s | 50 → 100 |
| Peak | 30s | 100 → 150 |
| Ramp down | 15s | 150 → 0 |

## Pass/Fail Thresholds

The test fails if any of these are breached:

| Metric | Threshold |
|---|---|
| 95th percentile response time | < 1000ms |
| HTTP error rate | < 5% |
| Hard errors (non-2xx) | < 50 total |
