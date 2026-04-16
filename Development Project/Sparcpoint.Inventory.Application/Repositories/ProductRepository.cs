using Sparcpoint.Inventory.Application.Interfaces;
using Sparcpoint.SqlServer.Abstractions;
using System.Data;

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
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"
INSERT INTO Instances.Products
(Name, Description, ProductImageUris, ValidSkus)
OUTPUT INSERTED.InstanceId
VALUES (@Name, @Description, @Images, @Skus);";

                AddParameter(cmd, "@Name", name);
                AddParameter(cmd, "@Description", description);
                AddParameter(cmd, "@Images", images);
                AddParameter(cmd, "@Skus", skus);

                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        });
    }

    public Task InsertAttributeAsync(int productId, string key, string value)
    {
        return _sql.ExecuteAsync(async (conn, tx) =>
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"
INSERT INTO Instances.ProductAttributes
(InstanceId, [Key], [Value])
VALUES (@ProductId, @Key, @Value);";

                AddParameter(cmd, "@ProductId", productId);
                AddParameter(cmd, "@Key", key);
                AddParameter(cmd, "@Value", value);

                cmd.ExecuteNonQuery();
                await Task.CompletedTask;
            }
        });
    }

    public Task InsertProductCategoryAsync(int productId, int categoryId)
    {
        return _sql.ExecuteAsync(async (conn, tx) =>
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"
INSERT INTO Instances.ProductCategories
(InstanceId, CategoryInstanceId)
VALUES (@ProductId, @CategoryId);";

                AddParameter(cmd, "@ProductId", productId);
                AddParameter(cmd, "@CategoryId", categoryId);

                cmd.ExecuteNonQuery();
                await Task.CompletedTask;
            }
        });
    }
    public Task<List<int>> SearchProductIdsAsync(Dictionary<string, string> attributes, List<int> categoryIds)
    {
        return _sql.ExecuteAsync(async (conn, tx) =>
        {
            var productIds = new List<int>();

            var cmd = conn.CreateCommand();
            cmd.Transaction = tx;

            var sql = @"
SELECT DISTINCT p.InstanceId
FROM Instances.Products p
WHERE 1 = 1
";

            if (categoryIds != null && categoryIds.Any())
            {
                sql += @"
AND p.InstanceId IN (
    SELECT pc.InstanceId
    FROM Instances.ProductCategories pc
    WHERE pc.CategoryInstanceId IN (" +
        string.Join(",", categoryIds) + @")
)";
            }

            cmd.CommandText = sql;

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                productIds.Add(reader.GetInt32(0));
            }

            reader.Close();

            if (attributes != null && attributes.Any())
            {
                var filtered = new List<int>();

                foreach (var id in productIds)
                {
                    var matchCmd = conn.CreateCommand();
                    matchCmd.Transaction = tx;

                    matchCmd.CommandText = @"
SELECT COUNT(*)
FROM Instances.ProductAttributes
WHERE InstanceId = @ProductId
AND ([Key] = @Key AND [Value] = @Value)";

                    foreach (var attr in attributes)
                    {
                        matchCmd.Parameters.Clear();

                        var p1 = matchCmd.CreateParameter();
                        p1.ParameterName = "@ProductId";
                        p1.Value = id;

                        var p2 = matchCmd.CreateParameter();
                        p2.ParameterName = "@Key";
                        p2.Value = attr.Key;

                        var p3 = matchCmd.CreateParameter();
                        p3.ParameterName = "@Value";
                        p3.Value = attr.Value;

                        matchCmd.Parameters.Add(p1);
                        matchCmd.Parameters.Add(p2);
                        matchCmd.Parameters.Add(p3);

                        var count = (int)matchCmd.ExecuteScalar();

                        if (count > 0)
                            filtered.Add(id);
                    }
                }

                productIds = filtered;
            }

            return productIds;
        });
    }

    private void AddParameter(IDbCommand cmd, string name, object value)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = name;
        param.Value = value ?? DBNull.Value;
        cmd.Parameters.Add(param);
    }
}