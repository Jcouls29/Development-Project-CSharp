using System.Data.SqlClient;
using System.Text;
using System.Text.Json;
using Dapper;
using Interview.Application.Abstractions;
using Interview.Application.UseCases.Command;
using Interview.Application.UseCases.Exception;
using Interview.Application.UseCases.Query;
using Interview.Application.UseCases.Result;
using Sparcpoint.SqlServer.Abstractions;

namespace Interview.Infrastructure.Sql;

public sealed class ProductRepository : IProductRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    private readonly ISqlExecutor _sqlExecutor;

    public ProductRepository(ISqlExecutor sqlExecutor)
    {
        _sqlExecutor = sqlExecutor;
    }

    public Task<bool> ProductNameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return _sqlExecutor.ExecuteAsync<bool>(async (connection, transaction) =>
        {
            const string sql = """
                SELECT CAST(CASE WHEN EXISTS (
                    SELECT 1
                    FROM [Instances].[Products]
                    WHERE [Name] = @Name
                ) THEN 1 ELSE 0 END AS BIT);
                """;

            var flag = await connection.ExecuteScalarAsync<int>(
                    new CommandDefinition(sql, new { Name = name }, transaction, cancellationToken: cancellationToken))
                .ConfigureAwait(false);

            return flag != 0;
        });
    }

    public Task<List<int>> GetMissingCategoryIdsAsync(
        IReadOnlyCollection<int> categoryIds,
        CancellationToken cancellationToken = default)
    {
        if (categoryIds.Count == 0)
        {
            return Task.FromResult(new List<int>());
        }

        return _sqlExecutor.ExecuteAsync<List<int>>(async (connection, transaction) =>
        {
            const string sql = """
                SELECT [InstanceId]
                FROM [Instances].[Categories]
                WHERE [InstanceId] IN @CategoryIds;
                """;

            var found = (await connection
                    .QueryAsync<int>(
                        new CommandDefinition(
                            sql,
                            new { CategoryIds = categoryIds.ToArray() },
                            transaction,
                            cancellationToken: cancellationToken))
                    .ConfigureAwait(false))
                .ToHashSet();

            var missing = categoryIds.Where(id => !found.Contains(id)).ToList();
            return missing;
        });
    }

    public Task<int> InsertProductAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        var productImageUrisJson = JsonSerializer.Serialize(command.ProductImageUris, JsonOptions);
        var validSkusJson = JsonSerializer.Serialize(command.ValidSkus, JsonOptions);

        return _sqlExecutor.ExecuteAsync<int>(async (connection, transaction) =>
        {
            int productId;
            try
            {
                const string insertProductSql = """
                    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus])
                    OUTPUT INSERTED.[InstanceId]
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                    """;

                productId = await connection.QuerySingleAsync<int>(
                        new CommandDefinition(
                            insertProductSql,
                            new
                            {
                                command.Name,
                                command.Description,
                                ProductImageUris = productImageUrisJson,
                                ValidSkus = validSkusJson,
                            },
                            transaction,
                            cancellationToken: cancellationToken))
                    .ConfigureAwait(false);
            }
            catch (SqlException ex) when (ex.Number is 2601 or 2627)
            {
                throw new DuplicateProductNameException();
            }

            const string insertAttributeSql = """
                INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
                VALUES (@InstanceId, @Key, @Value);
                """;

            foreach (var attribute in command.Attributes)
            {
                await connection.ExecuteAsync(
                        new CommandDefinition(
                            insertAttributeSql,
                            new { InstanceId = productId, attribute.Key, attribute.Value },
                            transaction,
                            cancellationToken: cancellationToken))
                    .ConfigureAwait(false);
            }

            const string insertCategorySql = """
                INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
                VALUES (@InstanceId, @CategoryInstanceId);
                """;

            foreach (var categoryId in command.CategoryIds)
            {
                await connection.ExecuteAsync(
                        new CommandDefinition(
                            insertCategorySql,
                            new { InstanceId = productId, CategoryInstanceId = categoryId },
                            transaction,
                            cancellationToken: cancellationToken))
                    .ConfigureAwait(false);
            }

            return productId;
        });
    }

    public Task<SearchProductsResult> SearchProductsAsync(
        SearchProductsQuery query,
        CancellationToken cancellationToken = default)
    {
        var offset = (query.Page - 1) * query.PageSize;
        var (whereClause, parameters) = BuildSearchWhereClause(query);
        parameters.Add("Offset", offset);
        parameters.Add("Fetch", query.PageSize);

        var countSql = $"""
            SELECT CAST(COUNT_BIG(1) AS INT)
            FROM [Instances].[Products] p
            WHERE {whereClause};
            """;

        var pageSql = $"""
            SELECT p.[InstanceId], p.[Name], p.[Description]
            FROM [Instances].[Products] p
            WHERE {whereClause}
            ORDER BY p.[InstanceId]
            OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY;
            """;

        return _sqlExecutor.ExecuteAsync<SearchProductsResult>(async (connection, transaction) =>
        {
            var total = await connection.ExecuteScalarAsync<int>(
                    new CommandDefinition(countSql, parameters, transaction, cancellationToken: cancellationToken))
                .ConfigureAwait(false);

            var rows = (await connection
                    .QueryAsync<ProductSummary>(
                        new CommandDefinition(pageSql, parameters, transaction, cancellationToken: cancellationToken))
                    .ConfigureAwait(false))
                .ToList();

            return new SearchProductsResult(rows, total, query.Page, query.PageSize);
        });
    }

    private static (string WhereClause, DynamicParameters Parameters) BuildSearchWhereClause(SearchProductsQuery query)
    {
        var parameters = new DynamicParameters();
        var conditions = new StringBuilder();

        if (!string.IsNullOrEmpty(query.SearchText))
        {
            var pattern = "%" + EscapeLikePattern(query.SearchText) + "%";
            parameters.Add("SearchPattern", pattern);
            conditions.Append("(p.[Name] LIKE @SearchPattern OR p.[Description] LIKE @SearchPattern)");
        }

        var categoryIds = query.CategoryIds ?? new List<int>();
        if (categoryIds.Count > 0)
        {
            if (conditions.Length > 0)
            {
                conditions.Append(" AND ");
            }

            var placeholders = new string[categoryIds.Count];
            for (var i = 0; i < categoryIds.Count; i++)
            {
                var name = $"Cat{i}";
                placeholders[i] = $"@{name}";
                parameters.Add(name, categoryIds[i]);
            }

            var inList = string.Join(", ", placeholders);
            conditions.Append(
                $"""
                EXISTS (
                    SELECT 1
                    FROM [Instances].[ProductCategories] pc
                    WHERE pc.[InstanceId] = p.[InstanceId]
                      AND pc.[CategoryInstanceId] IN ({inList})
                )
                """);
        }

        var attributeFilters = query.AttributeFilters ?? new List<AttributeFilterPair>();
        for (var i = 0; i < attributeFilters.Count; i++)
        {
            if (conditions.Length > 0)
            {
                conditions.Append(" AND ");
            }

            var keyName = $"AttrK{i}";
            var valueName = $"AttrV{i}";
            parameters.Add(keyName, attributeFilters[i].Key);
            parameters.Add(valueName, attributeFilters[i].Value);
            conditions.Append(
                $"""
                EXISTS (
                    SELECT 1
                    FROM [Instances].[ProductAttributes] pa
                    WHERE pa.[InstanceId] = p.[InstanceId]
                      AND pa.[Key] = @{keyName}
                      AND pa.[Value] = @{valueName}
                )
                """);
        }

        var whereClause = conditions.Length == 0 ? "1 = 1" : conditions.ToString();
        return (whereClause, parameters);
    }

    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace("[", "[[]", StringComparison.Ordinal)
            .Replace("%", "[%]", StringComparison.Ordinal)
            .Replace("_", "[_]", StringComparison.Ordinal);
    }
}
