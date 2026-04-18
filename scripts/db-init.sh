#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SQL_PROJECT_DIR="$ROOT_DIR/Development Project/Sparcpoint.Inventory.Database"

if ! command -v docker >/dev/null 2>&1; then
  echo "docker is required"
  exit 1
fi

if [[ ! -f "$ROOT_DIR/dev.env" ]]; then
  echo "Missing dev.env. Create it from dev.env.example first."
  exit 1
fi

SA_PASSWORD="$(grep '^MSSQL_SA_PASSWORD=' "$ROOT_DIR/dev.env" | sed 's/^MSSQL_SA_PASSWORD=//')"
if [[ -z "$SA_PASSWORD" ]]; then
  echo "MSSQL_SA_PASSWORD is empty in dev.env"
  exit 1
fi

if ! docker ps --format '{{.Names}}' | grep -q '^inventory-sql$'; then
  echo "Container inventory-sql is not running. Start it with: docker compose up -d"
  exit 1
fi

echo "Creating database 'inventory' if missing..."
docker exec inventory-sql /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$SA_PASSWORD" -C \
  -Q "IF DB_ID('inventory') IS NULL CREATE DATABASE [inventory];"

SQL_FILES=(
  "Instances/Instances.sql"
  "Instances/Tables/Categories.sql"
  "Instances/Tables/Products.sql"
  "Transactions/Transactions.sql"
  "Instances/Tables/CategoryAttributes.sql"
  "Instances/Tables/ProductAttributes.sql"
  "Instances/Tables/ProductCategories.sql"
  "Instances/Tables/CategoryCategories.sql"
  "Table Types/IntegerList.sql"
  "Table Types/CustomAttributeList.sql"
  "Table Types/StringList.sql"
  "Transactions/Tables/InventoryTransactions.sql"
  "Table Types/CorrelatedCustomAttributeList.sql"
  "Table Types/CorrelatedIntegerList.sql"
  "Table Types/CorrelatedStringList.sql"
  "Instances/Table Types/CorrelatedProductInstanceList.sql"
  "Instances/Table Types/CorrelatedListItemList.sql"
)

echo "Copying SQL scripts into container..."
docker exec inventory-sql mkdir -p /tmp/inventory-db
for relative_file in "${SQL_FILES[@]}"; do
  host_file="$SQL_PROJECT_DIR/$relative_file"
  container_dir="/tmp/inventory-db/$(dirname "$relative_file")"
  if [[ ! -f "$host_file" ]]; then
    echo "Missing SQL file: $host_file"
    exit 1
  fi
  docker exec inventory-sql mkdir -p "$container_dir"
  docker cp "$host_file" "inventory-sql:/tmp/inventory-db/$relative_file"
done

echo "Applying schema to inventory database..."
for relative_file in "${SQL_FILES[@]}"; do
  echo " - $relative_file"
  docker exec inventory-sql /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "$SA_PASSWORD" -C -b -d inventory \
    -i "/tmp/inventory-db/$relative_file"
done

echo "Verifying tables..."
docker exec inventory-sql /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$SA_PASSWORD" -C -d inventory -W -s "|" \
  -Q "SELECT s.name AS [schema], t.name AS [table] FROM sys.tables t INNER JOIN sys.schemas s ON s.schema_id=t.schema_id WHERE s.name IN ('Instances','Transactions') ORDER BY s.name, t.name;"

echo "Done. Database inventory is ready."
