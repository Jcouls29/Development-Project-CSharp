using Dapper;
using Sparcpoint;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _sqlExecutor;
        private readonly IDataSerializer _dataSerializer;

        public ProductRepository(ISqlExecutor sqlExecutor, IDataSerializer dataSerializer)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
            _dataSerializer = dataSerializer ?? throw new ArgumentNullException(nameof(dataSerializer));
        }

        public Task<int> CreateAsync(CreateProductRequest request)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var instanceId = await connection.ExecuteScalarAsync<int>(@"
INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus])
OUTPUT INSERTED.[InstanceId]
VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);",
                    new
                    {
                        request.Name,
                        request.Description,
                        ProductImageUris = SerializeList(request.ProductImageUris),
                        ValidSkus = SerializeList(request.ValidSkus),
                    }, transaction);

                foreach (var attribute in request.Attributes ?? new Dictionary<string, string>())
                {
                    await connection.ExecuteAsync(@"
INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
VALUES (@InstanceId, @Key, @Value);",
                        new
                        {
                            InstanceId = instanceId,
                            Key = attribute.Key,
                            Value = attribute.Value,
                        }, transaction);
                }

                foreach (var categoryId in request.CategoryIds ?? new List<int>())
                {
                    await connection.ExecuteAsync(@"
INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
VALUES (@InstanceId, @CategoryInstanceId);",
                        new
                        {
                            InstanceId = instanceId,
                            CategoryInstanceId = categoryId,
                        }, transaction);
                }

                return instanceId;
            });
        }

        public Task<ProductDetailModel> GetByIdAsync(int instanceId)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                using (var grid = await connection.QueryMultipleAsync(@"
SELECT
    p.[InstanceId],
    p.[Name],
    p.[Description],
    p.[ProductImageUris],
    p.[ValidSkus],
    p.[CreatedTimestamp]
FROM [Instances].[Products] p
WHERE p.[InstanceId] = @InstanceId;

SELECT
    pa.[Key],
    pa.[Value]
FROM [Instances].[ProductAttributes] pa
WHERE pa.[InstanceId] = @InstanceId;

SELECT
    c.[InstanceId],
    c.[Name],
    c.[Description],
    c.[CreatedTimestamp]
FROM [Instances].[ProductCategories] pc
INNER JOIN [Instances].[Categories] c ON c.[InstanceId] = pc.[CategoryInstanceId]
WHERE pc.[InstanceId] = @InstanceId
ORDER BY c.[Name] ASC;",
                    new { InstanceId = instanceId }, transaction))
                {
                    var row = await grid.ReadSingleOrDefaultAsync<ProductRow>();

                    if (row == null)
                        return null;

                    var attributes = (await grid.ReadAsync<ProductAttributeRow>())
                        .ToDictionary(item => item.Key, item => item.Value);

                    var categories = (await grid.ReadAsync<CategoryModel>()).ToList();

                    return new ProductDetailModel
                    {
                        InstanceId = row.InstanceId,
                        Name = row.Name,
                        Description = row.Description,
                        ProductImageUris = DeserializeList(row.ProductImageUris),
                        ValidSkus = DeserializeList(row.ValidSkus),
                        CreatedTimestamp = row.CreatedTimestamp,
                        Attributes = attributes,
                        Categories = categories,
                    };
                }
            });
        }

        public Task<PaginatedResult<ProductModel>> SearchAsync(ProductSearchRequest request)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var queryProvider = new SqlServerQueryProvider()
                    .SetTargetTableAlias("p")
                    .OrderByAscending("CreatedTimestamp");

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    var parameterName = queryProvider.GetNextParameterName("@Name");
                    queryProvider.Where($"p.[Name] LIKE {parameterName}");
                    queryProvider.AddParameter(parameterName, $"%{request.Name}%");
                }

                if (!string.IsNullOrWhiteSpace(request.Description))
                {
                    var parameterName = queryProvider.GetNextParameterName("@Description");
                    queryProvider.Where($"p.[Description] LIKE {parameterName}");
                    queryProvider.AddParameter(parameterName, $"%{request.Description}%");
                }

                AddAttributeFilters(queryProvider, request.Attributes);
                AddCategoryFilters(queryProvider, request.CategoryIds);

                var sql = new StringBuilder();
                sql.AppendLine("SELECT COUNT(*)");
                sql.AppendLine("FROM [Instances].[Products] p");

                if (!string.IsNullOrWhiteSpace(queryProvider.JoinClause))
                    sql.AppendLine(queryProvider.JoinClause);

                if (!string.IsNullOrWhiteSpace(queryProvider.WhereClause))
                    sql.AppendLine(queryProvider.WhereClause + ";");
                else
                    sql.AppendLine(";");

                sql.AppendLine();
                sql.AppendLine("SELECT");
                sql.AppendLine("    p.[InstanceId],");
                sql.AppendLine("    p.[Name],");
                sql.AppendLine("    p.[Description],");
                sql.AppendLine("    p.[ProductImageUris],");
                sql.AppendLine("    p.[ValidSkus],");
                sql.AppendLine("    p.[CreatedTimestamp]");
                sql.AppendLine("FROM [Instances].[Products] p");

                if (!string.IsNullOrWhiteSpace(queryProvider.JoinClause))
                    sql.AppendLine(queryProvider.JoinClause);

                if (!string.IsNullOrWhiteSpace(queryProvider.WhereClause))
                    sql.AppendLine(queryProvider.WhereClause);

                sql.AppendLine(queryProvider.OrderByClause);
                sql.AppendLine("OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;");

                var parameters = new DynamicParameters();

                foreach (var parameter in queryProvider.Parameters)
                    parameters.Add(parameter.Key, parameter.Value);

                parameters.Add("@Offset", (request.Page - 1) * request.PageSize);
                parameters.Add("@PageSize", request.PageSize);

                using (var grid = await connection.QueryMultipleAsync(sql.ToString(), parameters, transaction))
                {
                    var totalCount = await grid.ReadSingleAsync<int>();
                    var items = (await grid.ReadAsync<ProductRow>())
                        .Select(MapProduct)
                        .ToList();

                    return new PaginatedResult<ProductModel>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        Page = request.Page,
                        PageSize = request.PageSize,
                    };
                }
            });
        }

        private void AddAttributeFilters(SqlServerQueryProvider queryProvider, Dictionary<string, string> attributes)
        {
            if (attributes == null)
                return;

            foreach (var attribute in attributes)
            {
                var keyParameter = queryProvider.GetNextParameterName("@AttributeKey");
                var valueParameter = queryProvider.GetNextParameterName("@AttributeValue");

                queryProvider.Where($@"EXISTS (
    SELECT 1
    FROM [Instances].[ProductAttributes] pa
    WHERE pa.[InstanceId] = p.[InstanceId]
      AND pa.[Key] = {keyParameter}
      AND pa.[Value] LIKE {valueParameter}
)");

                queryProvider.AddParameter(keyParameter, attribute.Key);
                queryProvider.AddParameter(valueParameter, $"%{attribute.Value}%");
            }
        }

        private void AddCategoryFilters(SqlServerQueryProvider queryProvider, List<int> categoryIds)
        {
            if (categoryIds == null || categoryIds.Count == 0)
                return;

            var parameterNames = new List<string>();

            foreach (var categoryId in categoryIds.Distinct())
            {
                var parameterName = queryProvider.GetNextParameterName("@CategoryId");
                queryProvider.AddParameter(parameterName, categoryId);
                parameterNames.Add(parameterName);
            }

            queryProvider.Where($@"EXISTS (
    SELECT 1
    FROM [Instances].[ProductCategories] pc
    WHERE pc.[InstanceId] = p.[InstanceId]
      AND pc.[CategoryInstanceId] IN ({string.Join(", ", parameterNames)})
)");
        }

        private ProductModel MapProduct(ProductRow row)
        {
            return new ProductModel
            {
                InstanceId = row.InstanceId,
                Name = row.Name,
                Description = row.Description,
                ProductImageUris = DeserializeList(row.ProductImageUris),
                ValidSkus = DeserializeList(row.ValidSkus),
                CreatedTimestamp = row.CreatedTimestamp,
            };
        }

        private string SerializeList(List<string> values)
            => _dataSerializer.Serialize(values ?? new List<string>());

        private List<string> DeserializeList(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new List<string>();

            return _dataSerializer.Deserialize<List<string>>(value) ?? new List<string>();
        }

        private class ProductRow
        {
            public int InstanceId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ProductImageUris { get; set; }
            public string ValidSkus { get; set; }
            public DateTime CreatedTimestamp { get; set; }
        }

        private class ProductAttributeRow
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}
