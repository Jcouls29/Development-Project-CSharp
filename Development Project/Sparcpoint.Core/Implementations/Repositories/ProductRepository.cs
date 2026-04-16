using Dapper;
using Interview.Web.Models;
using Sparcpoint.Abstract.Repositories;
using Sparcpoint.Entities;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        public async Task<Product> AddAsync(Product product)
        {
            var provider = new SqlServerQueryProvider()
                .OrderBy("InstanceId", OrderByClause.OrderByDirection.Descending);

            var attrsQry = $@"
-- Insert product attributes
INSERT INTO Instances.ProductAttributes (InstanceId, [Key], [Value]) VALUES
{string.Join(", \n", product.Attributes.Select(a => $"(@LastId, '{a.Key}', '{a.Value}')"))};
";

            var cmdStr = $@"
-- Insert new Product
INSERT INTO Instances.Products (Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp) 
VALUES (@Name, @Description, @ProductImageUris, @ValidSkus, SYSUTCDATETIME());

DECLARE @LastId INT = SCOPE_IDENTITY();

{(product.Attributes.Any() ? attrsQry : "")}

-- Get last item inserted
SELECT TOP 1 * FROM Instances.Products {provider.OrderByClause};
";

            return await _sqlExecutor.ExecuteAsync<Product>(async (conn, trans) =>
            {
                using (var cmd = new SqlCommand(cmdStr, (SqlConnection)conn, (SqlTransaction)trans))
                {
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@ProductImageUris", product.ProductImageUris);
                    cmd.Parameters.AddWithValue("@ValidSkus", product.ValidSkus);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Product
                            {
                                InstanceId = reader.GetInt32(reader.GetOrdinal("InstanceId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                ProductImageUris = reader.GetString(reader.GetOrdinal("ProductImageUris")),
                                ValidSkus = reader.GetString(reader.GetOrdinal("ValidSkus")),
                                CreatedTimestamp = reader.GetDateTime(reader.GetOrdinal("CreatedTimestamp"))
                            };
                        }

                        return null;
                    }
                }
            });

        }

        public Task<Product> GetByIdAsync(int id, CancellationToken ct = default)
        {
            const string productQuery = @"
SELECT * FROM Instances.Products WHERE InstanceId = @InstanceId;
";

            const string productAttrsQuery = @"
SELECT * FROM Instances.ProductAttributes WHERE InstanceId = @InstanceId;
";

            return _sqlExecutor.ExecuteAsync<Product>(async (conn, trans) =>
            {
                var command = new CommandDefinition(
                    productQuery,
                    new { InstanceId = id },
                    trans,
                    cancellationToken: ct);

                var product = await conn.QuerySingleOrDefaultAsync<Product>(command);

                if(product is null)
                {
                    return null;
                }

                command = new CommandDefinition(
                    productAttrsQuery,
                    new { InstanceId = id },
                    trans,
                    cancellationToken: ct);

                // EVAL: Attributes
                var attrs = (await conn.QueryAsync<ProductAttribute>(command)).ToArray();

                product.Attributes = attrs;

                return product;
            });
        }

        public async Task<List<Product>> SearchAsync(SearchRequest search, CancellationToken ct = default)
        {
            var attrsFilter = search.ParseAttrsFilters();

            var attrsQuery = !attrsFilter.Any() ? "" 
                : $" AND ({string.Join(" AND ", attrsFilter.Select(a => $"(PA.[Key] = '{a.Key}' OR PA.[Value] = '{a.Value}')"))})";

            var query = $@"
SELECT TOP({search.Take})
    P.InstanceId, P.Name, P.Description, P.ProductImageUris, P.ValidSkus, P.CreatedTimestamp,
    PA.[Key], PA.[Value]
FROM Instances.Products P
JOIN Instances.ProductAttributes PA
    ON P.InstanceId = PA.InstanceId
WHERE (@Name IS NULL OR P.Name LIKE '%' + @Name + '%')
AND (@Description IS NULL OR P.Description LIKE '%' + @Description + '%')
AND (@Sku IS NULL OR P.ValidSkus LIKE '%' + @Sku + '%')
{attrsQuery}
;
";
            return await _sqlExecutor.ExecuteAsync<List<Product>>(async (conn, trans) =>
            {
                var command = new CommandDefinition(
                    query,
                    new 
                    { 
                        Name = search.GetFilterValueAsString("name"),
                        Description = search.GetFilterValueAsString("description"),
                        Sku = search.GetFilterValueAsString("sku")
                    },
                    trans,
                    cancellationToken: ct);

                var products = (await conn.QueryAsync<Product>(command)).ToList();

                return products;
            });
        }
    }
}
