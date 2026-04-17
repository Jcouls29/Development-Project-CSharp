using Dapper;
using Sparcpoint.Inventory.Application.Interfaces;
using Sparcpoint.SqlServer.Abstractions;

public class ProductRepository : IProductRepository
{
    private readonly ISqlExecutor _sql;

    public ProductRepository(ISqlExecutor sql)
    {
        _sql = sql;
    }

    public Task<int> InsertProductAsync(string name, string description, string images, string skus)
    {
        return _sql.ExecuteAsync(async (conn, tx) =>
        {
            var query = @"
INSERT INTO Instances.Products
(Name, Description, ProductImageUris, ValidSkus)
OUTPUT INSERTED.InstanceId
VALUES (@Name, @Description, @Images, @Skus);";

            return await conn.ExecuteScalarAsync<int>(
                query,
                new { Name = name, Description = description, Images = images, Skus = skus },
                tx
            );
        });
    }

    public Task InsertAttributeAsync(int productId, string key, string value)
    {
        return _sql.ExecuteAsync((conn, tx) =>
        {
            var query = @"
INSERT INTO Instances.ProductAttributes
(InstanceId, [Key], [Value])
VALUES (@ProductId, @Key, @Value);";

            return conn.ExecuteAsync(
                query,
                new { ProductId = productId, Key = key, Value = value },
                tx
            );
        });
    }

    public Task InsertProductCategoryAsync(int productId, int categoryId)
    {
        return _sql.ExecuteAsync((conn, tx) =>
        {
            var query = @"
INSERT INTO Instances.ProductCategories
(InstanceId, CategoryInstanceId)
VALUES (@ProductId, @CategoryId);";

            return conn.ExecuteAsync(
                query,
                new { ProductId = productId, CategoryId = categoryId },
                tx
            );
        });
    }

    public Task<List<int>> SearchProductIdsAsync(
    Dictionary<string, string> attributes,
    List<int> categoryIds)
    {
        return _sql.ExecuteAsync(async (conn, tx) =>
        {
            var sql = @"
SELECT DISTINCT p.InstanceId
FROM Instances.Products p
WHERE 1 = 1";

            if (categoryIds?.Any() == true)
            {
                sql += @"
AND p.InstanceId IN (
    SELECT InstanceId
    FROM Instances.ProductCategories
    WHERE CategoryInstanceId IN @CategoryIds
)";
            }

            var productIds = (await conn.QueryAsync<int>(
                sql,
                new { CategoryIds = categoryIds },
                tx
            )).ToList();

            if (attributes?.Any() == true && productIds.Any())
            {
                var attrSql = @"
SELECT DISTINCT InstanceId
FROM Instances.ProductAttributes
WHERE (InstanceId IN @Ids)
AND ([Key] = @Key AND [Value] = @Value)";

                foreach (var attr in attributes)
                {
                    productIds = (await conn.QueryAsync<int>(
                        attrSql,
                        new { Ids = productIds, Key = attr.Key, Value = attr.Value },
                        tx
                    )).ToList();
                }
            }

            return productIds;
        });
    }
}