# C# Inventory API Challenge

Short overview for setting up and running the project locally, plus the features implemented in this submission.

## Tech Stack
- ASP.NET Core Web API
- SQL Server
- Docker Compose for local database
- .NET 8 target framework

## Prerequisites
- Docker Desktop
- .NET SDK (compatible with net8.0)

## Project Setup
1. Create local environment file:

```bash
cp dev.env.example dev.env
```

2. Start SQL Server:

```bash
docker compose up -d
```

3. Initialize database schema:

```bash
chmod +x scripts/db-init.sh
./scripts/db-init.sh
```

4. Confirm container is healthy:

```bash
docker compose ps
```

## Run the API

From the repository root:

- Build:

```bash
dotnet build "Development Project/Interview.Web/Interview.Web.csproj"
```

- Run:

```bash
dotnet run --project "Development Project/Interview.Web/Interview.Web.csproj"
```

Default local URL:
- http://localhost:4000

## Implemented Features

### 1) Add Product
- Endpoint: POST /api/v1/products
- Creates a product with:
  - Base fields (name, description)
  - Optional image URIs and valid SKUs
  - Metadata key/value pairs
  - Category associations
- Includes validation and clear bad-request responses.

### 2) Search Products
- Endpoint: GET /api/v1/products/search
- Supports optional filters:
  - General text (name/description)
  - Metadata key/value
  - Category IDs
- Returns hydrated product payloads including metadata and categories.

### 3) Inventory Operations (Lean Slice)
- Endpoint: POST /api/v1/products/{productId}/inventory/add
- Endpoint: POST /api/v1/products/{productId}/inventory/remove
- Endpoint: GET /api/v1/products/{productId}/inventory/count
- Includes validation for productId and quantity.
