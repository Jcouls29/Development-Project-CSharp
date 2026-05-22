using Dapper;
using Sparcpoint;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Implementations;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Inventory.IntegrationTests.Infrastructure
{
    // EVAL: Spins up a fresh, isolated LocalDB database for each test run, applies the same schema
    // defined in the Sparcpoint.Inventory.Database project, and tears it down on disposal. Tests
    // remain hermetic and reviewers can simply run `dotnet test` from the solution root — no
    // separate publish step required for the test database.
    public sealed class TestDatabaseFixture : IAsyncLifetime
    {
        private const string MasterConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=master;Integrated Security=true;";

        private readonly string _DatabaseName;
        public string ConnectionString { get; }
        public ISqlExecutor Executor { get; private set; }
        public IDataSerializer Serializer { get; } = new JsonDataSerializer();

        public TestDatabaseFixture()
        {
            _DatabaseName = "SparcpointInventory_Test_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            ConnectionString = $"Server=(localdb)\\MSSQLLocalDB;Database={_DatabaseName};Integrated Security=true;";
        }

        public IProductRepository CreateProductRepository() => new SqlProductRepository(Executor, Serializer);
        public ICategoryRepository CreateCategoryRepository() => new SqlCategoryRepository(Executor);
        public IInventoryRepository CreateInventoryRepository() => new SqlInventoryRepository(Executor);

        // EVAL: Called from each test's InitializeAsync so tests never see data from a sibling
        // test. Cheaper than rebuilding the schema, and CASCADE constraints handle the dependent
        // rows so explicit deletion order isn't required.
        public async Task ResetAsync()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                await conn.ExecuteAsync(@"
                    DELETE FROM [Transactions].[InventoryTransactions];
                    DELETE FROM [Instances].[ProductAttributes];
                    DELETE FROM [Instances].[CategoryAttributes];
                    DELETE FROM [Instances].[ProductCategories];
                    DELETE FROM [Instances].[CategoryCategories];
                    DELETE FROM [Instances].[Products];
                    DELETE FROM [Instances].[Categories];");
            }
        }

        public async Task InitializeAsync()
        {
            using (var conn = new SqlConnection(MasterConnectionString))
            {
                await conn.OpenAsync();
                await conn.ExecuteAsync($"CREATE DATABASE [{_DatabaseName}]");
            }

            Executor = new SqlServerExecutor(ConnectionString);
            await ApplySchemaAsync();
        }

        public async Task DisposeAsync()
        {
            using (var conn = new SqlConnection(MasterConnectionString))
            {
                await conn.OpenAsync();
                await conn.ExecuteAsync(
                    $"ALTER DATABASE [{_DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{_DatabaseName}];");
            }
        }

        private async Task ApplySchemaAsync()
        {
            // EVAL: Schema is embedded here (rather than loaded from the .sqlproj at runtime) so
            // tests are self-contained. If the production schema evolves, update this single block
            // — kept minimal: only the objects the inventory layer actually reads from or writes to.
            using (var conn = new SqlConnection(ConnectionString))
            {
                await conn.OpenAsync();

                foreach (var batch in SchemaBatches)
                    await conn.ExecuteAsync(batch);
            }
        }

        private static readonly string[] SchemaBatches = new[]
        {
            "CREATE SCHEMA [Instances]",
            "CREATE SCHEMA [Transactions]",

            @"CREATE TYPE [dbo].[IntegerList] AS TABLE ([Value] INT NOT NULL)",
            @"CREATE TYPE [dbo].[CustomAttributeList] AS TABLE (
                [Key] VARCHAR(64) NOT NULL,
                [Value] VARCHAR(512) NOT NULL)",

            @"CREATE TABLE [Instances].[Products] (
                [InstanceId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
                [Name] VARCHAR(256) NOT NULL,
                [Description] VARCHAR(256) NOT NULL,
                [ProductImageUris] VARCHAR(MAX) NOT NULL,
                [ValidSkus] VARCHAR(MAX) NOT NULL,
                [CreatedTimestamp] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME())",

            @"CREATE TABLE [Instances].[Categories] (
                [InstanceId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
                [Name] VARCHAR(64) NOT NULL,
                [Description] VARCHAR(256) NOT NULL,
                [CreatedTimestamp] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME())",

            @"CREATE TABLE [Instances].[ProductAttributes] (
                [InstanceId] INT NOT NULL,
                [Key] VARCHAR(64) NOT NULL,
                [Value] VARCHAR(512) NOT NULL,
                CONSTRAINT [PK_ProductAttributes] PRIMARY KEY ([InstanceId], [Key]),
                CONSTRAINT [FK_ProductAttributes_Products] FOREIGN KEY ([InstanceId])
                    REFERENCES [Instances].[Products]([InstanceId]) ON DELETE CASCADE)",

            @"CREATE INDEX [IX_ProductAttributes_Key_Value]
                ON [Instances].[ProductAttributes] ([Key] ASC, [Value] ASC)",

            @"CREATE TABLE [Instances].[CategoryAttributes] (
                [InstanceId] INT NOT NULL,
                [Key] VARCHAR(64) NOT NULL,
                [Value] VARCHAR(512) NOT NULL,
                CONSTRAINT [PK_CategoryAttributes] PRIMARY KEY ([InstanceId], [Key]),
                CONSTRAINT [FK_CategoryAttributes_Categories] FOREIGN KEY ([InstanceId])
                    REFERENCES [Instances].[Categories]([InstanceId]) ON DELETE CASCADE)",

            @"CREATE TABLE [Instances].[ProductCategories] (
                [InstanceId] INT NOT NULL,
                [CategoryInstanceId] INT NOT NULL,
                CONSTRAINT [PK_ProductCategories] PRIMARY KEY ([InstanceId], [CategoryInstanceId]),
                CONSTRAINT [FK_ProductCategories_Products] FOREIGN KEY ([InstanceId])
                    REFERENCES [Instances].[Products]([InstanceId]) ON DELETE CASCADE,
                CONSTRAINT [FK_ProductCategories_Categories] FOREIGN KEY ([CategoryInstanceId])
                    REFERENCES [Instances].[Categories]([InstanceId]) ON DELETE CASCADE)",

            @"CREATE TABLE [Instances].[CategoryCategories] (
                [InstanceId] INT NOT NULL,
                [CategoryInstanceId] INT NOT NULL,
                CONSTRAINT [PK_CategoryCategories] PRIMARY KEY ([InstanceId], [CategoryInstanceId]),
                CONSTRAINT [FK_CategoryCategories_Categories] FOREIGN KEY ([InstanceId])
                    REFERENCES [Instances].[Categories]([InstanceId]) ON DELETE CASCADE,
                CONSTRAINT [FK_CategoryCategories_Categories_Categories] FOREIGN KEY ([CategoryInstanceId])
                    REFERENCES [Instances].[Categories]([InstanceId]))",

            @"CREATE TABLE [Transactions].[InventoryTransactions] (
                [TransactionId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
                [ProductInstanceId] INT NOT NULL,
                [Quantity] DECIMAL(19,6) NOT NULL,
                [StartedTimestamp] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
                [CompletedTimestamp] DATETIME2(7) NULL,
                [TypeCategory] VARCHAR(32) NULL,
                CONSTRAINT [FK_InventoryTransactions_Products] FOREIGN KEY ([ProductInstanceId])
                    REFERENCES [Instances].[Products]([InstanceId]) ON DELETE CASCADE)"
        };
    }

    [CollectionDefinition(Name)]
    public class TestDatabaseCollection : ICollectionFixture<TestDatabaseFixture>
    {
        public const string Name = "Integration database collection";
    }
}
