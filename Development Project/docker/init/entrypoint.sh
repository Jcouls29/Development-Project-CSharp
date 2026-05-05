#!/bin/bash
# Wait for SQL Server to be ready, then run the idempotent init script
echo "Running database initialization script..."
/opt/mssql-tools18/bin/sqlcmd -S sqlserver -U sa -P "Sparcpoint#2024!" -C -i /docker-init/init.sql
echo "Database initialization complete."
