using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Interview.Web.Data
{
    public class DatabaseSeeder
    {
        private readonly ISqlExecutor _sqlExecutor;

        public DatabaseSeeder(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public async Task SeedAsync()
        {
            await SeedProductsAsync();
            await SeedCategoriesAsync();
            await SeedCategoryAttributesAsync();
            await SeedCategoryCategoriesAsync();
        }

        private async Task SeedProductsAsync()
        {
            await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                using var checkCmd = connection.CreateCommand();
                checkCmd.Transaction = transaction;
                checkCmd.CommandText = "SELECT COUNT(*) FROM Instances.Products";
                var count = (int)await ExecuteScalarAsync(checkCmd);

                if (count > 0)
                    return;

                var products = new[]
                {
                    ("Product A", "Description for Product A", "https://example.com/images/product-a.jpg", "SKU-001,SKU-002"),
                    ("Product B", "Description for Product B", "https://example.com/images/product-b.jpg", "SKU-003"),
                    ("Product C", "Description for Product C", "https://example.com/images/product-c.jpg", "SKU-004,SKU-005,SKU-006"),
                };

                foreach (var (name, description, imageUris, skus) in products)
                {
                    using var cmd = connection.CreateCommand();
                    cmd.Transaction = transaction;
                    cmd.CommandText = @"
                        INSERT INTO Instances.Products (Name, Description, ProductImageUris, ValidSkus)
                        VALUES (@Name, @Description, @ProductImageUris, @ValidSkus)";

                    AddParameter(cmd, "@Name", name);
                    AddParameter(cmd, "@Description", description);
                    AddParameter(cmd, "@ProductImageUris", imageUris);
                    AddParameter(cmd, "@ValidSkus", skus);

                    await ExecuteNonQueryAsync(cmd);
                }
            });
        }

        private async Task SeedCategoriesAsync()
        {
            await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                using var checkCmd = connection.CreateCommand();
                checkCmd.Transaction = transaction;
                checkCmd.CommandText = "SELECT COUNT(*) FROM Instances.Categories";
                var count = (int)await ExecuteScalarAsync(checkCmd);

                if (count > 0)
                    return;

                var categories = new[]
                {
                    (1, "Electronics", "Electronic devices and accessories"),
                    (2, "Clothing", "Apparel and fashion items"),
                    (3, "Home & Garden", "Home improvement and garden supplies"),
                    (4, "Smartphones", "Mobile phones and accessories"),
                    (5, "Laptops", "Portable computers"),
                };

                foreach (var (id, name, description) in categories)
                {
                    using var cmd = connection.CreateCommand();
                    cmd.Transaction = transaction;
                    cmd.CommandText = @"
                        SET IDENTITY_INSERT Instances.Categories ON;
                        INSERT INTO Instances.Categories (InstanceId, Name, Description)
                        VALUES (@InstanceId, @Name, @Description);
                        SET IDENTITY_INSERT Instances.Categories OFF;";

                    AddParameter(cmd, "@InstanceId", id);
                    AddParameter(cmd, "@Name", name);
                    AddParameter(cmd, "@Description", description);

                    await ExecuteNonQueryAsync(cmd);
                }
            });
        }

        private async Task SeedCategoryAttributesAsync()
        {
            await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                using var checkCmd = connection.CreateCommand();
                checkCmd.Transaction = transaction;
                checkCmd.CommandText = "SELECT COUNT(*) FROM Instances.CategoryAttributes";
                var count = (int)await ExecuteScalarAsync(checkCmd);

                if (count > 0)
                    return;

                var attributes = new[]
                {
                    (1, "Warranty", "Warranty period information"),
                    (1, "Voltage", "Operating voltage"),
                    (2, "Material", "Fabric or material type"),
                    (2, "Size", "Available sizes"),
                    (3, "Weight", "Product weight"),
                    (4, "Screen Size", "Display size in inches"),
                    (5, "RAM", "Memory capacity"),
                };

                foreach (var (categoryId, name, description) in attributes)
                {
                    using var cmd = connection.CreateCommand();
                    cmd.Transaction = transaction;
                    cmd.CommandText = @"
                        INSERT INTO Instances.CategoryAttributes (InstanceId, [Key], [Value])
                        VALUES (@InstanceId, @Key, @Value)";

                    AddParameter(cmd, "@InstanceId", categoryId);
                    AddParameter(cmd, "@Key", name);
                    AddParameter(cmd, "@Value", description);

                    await ExecuteNonQueryAsync(cmd);
                }
            });
        }

        private async Task SeedCategoryCategoriesAsync()
        {
            await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                using var checkCmd = connection.CreateCommand();
                checkCmd.Transaction = transaction;
                checkCmd.CommandText = "SELECT COUNT(*) FROM Instances.CategoryCategories";
                var count = (int)await ExecuteScalarAsync(checkCmd);

                if (count > 0)
                    return;

                var relationships = new[]
                {
                    (1, 4),
                    (1, 5),
                };

                foreach (var (parentCategoryId, childCategoryId) in relationships)
                {
                    using var cmd = connection.CreateCommand();
                    cmd.Transaction = transaction;
                    cmd.CommandText = @"
                        INSERT INTO Instances.CategoryCategories (InstanceId, CategoryInstanceId)
                        VALUES (@InstanceId, @CategoryInstanceId)";

                    AddParameter(cmd, "@InstanceId", parentCategoryId);
                    AddParameter(cmd, "@CategoryInstanceId", childCategoryId);
                    await ExecuteNonQueryAsync(cmd);
                }
            });
        }

        private static void AddParameter(IDbCommand cmd, string name, string value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name;
            param.Value = (object)value ?? DBNull.Value;
            cmd.Parameters.Add(param);
        }

        private static void AddParameter(IDbCommand cmd, string name, int value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name;
            param.Value = value;
            cmd.Parameters.Add(param);
        }

        private static Task ExecuteNonQueryAsync(IDbCommand cmd)
        {
            if (cmd is System.Data.Common.DbCommand dbCmd)
                return dbCmd.ExecuteNonQueryAsync();

            cmd.ExecuteNonQuery();
            return Task.CompletedTask;
        }

        private static Task<object> ExecuteScalarAsync(IDbCommand cmd)
        {
            if (cmd is System.Data.Common.DbCommand dbCmd)
                return dbCmd.ExecuteScalarAsync();

            return Task.FromResult(cmd.ExecuteScalar());
        }
    }
}
