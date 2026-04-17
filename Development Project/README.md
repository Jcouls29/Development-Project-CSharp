Interview Demo - Setup
======================

This repository contains a small interview demo for an inventory system.

Quick start (local):

1. Ensure .NET 8 SDK/runtime is installed.
2. Create a SQL Server database (e.g. InterviewDemo) and run sql/create_tables.sql to create the minimal schema.
3. Update Interview.Web/appsettings.json -> SqlServerOptions.ConnectionString with your database connection string.
4. From repository root (PowerShell):
   dotnet restore
   dotnet build
   dotnet run --project "\"Interview.Web\Interview.Web.csproj\"" --urls "http://localhost:5000"
5. Open Swagger at http://localhost:5000/swagger to test endpoints.

Notes:
- The project includes a minimal SqlProductRepository and SqlInventoryRepository using the provided ISqlExecutor abstraction.
- This is an interview exercise; further validation and features should be added as needed.
