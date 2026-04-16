using Dapper;
using Newtonsoft.Json;
using Sparcpoint.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Infrastucture
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public async Task<int> AddAsync(Product product)
        {
            // EVAL: Validate pre-conditions using the class provided in the Core
            PreConditions.ParameterNotNull(product, nameof(product));
            PreConditions.StringNotNullOrWhitespace(product.Name, nameof(product.Name));

            // 1. Insert Product into [Instances].[Products]
            const string sqlProduct = @"
                    INSERT INTO [Instances].[Products] (Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp) 
                    OUTPUT INSERTED.InstanceId 
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus, SYSUTCDATETIME());";

            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                int instanceId = await connection.QuerySingleAsync<int>(sqlProduct, product, transaction);

                // 2. Insert Categories
                if (product.CategoryInstanceIds != null && product.CategoryInstanceIds.Any())
                {
                    const string sqlCat = "INSERT INTO [Instances].[ProductCategories] (InstanceId, CategoryInstanceId) VALUES (@instanceId, @catId);";
                    await connection.ExecuteAsync(sqlCat, product.CategoryInstanceIds.Select(cId => new { instanceId, catId = cId }), transaction);
                }

                // 3. Insert Attributes
                if (product.Attributes != null && product.Attributes.Any())
                {
                    const string sqlAttr = "INSERT INTO [Instances].[ProductAttributes] (InstanceId, [Key], [Value]) VALUES (@instanceId, @key, @value);";
                    await connection.ExecuteAsync(sqlAttr, product.Attributes.Select(a => new { instanceId, key = a.Key, value = a.Value }), transaction);
                }

                product.InstanceId = instanceId;
                return instanceId;
            });
        }

        public async Task<IEnumerable<Product>> SearchAsync(ProductSearchRequest request)
        {
            var parameters = new DynamicParameters();
            var sql = new StringBuilder();

            sql.Append(@"
                ;WITH CategoryTree AS (
                    SELECT InstanceId FROM [Instances].[Categories] WHERE InstanceId IN @CatIds
                    UNION ALL
                    SELECT cc.CategoryInstanceId
                    FROM [Instances].[CategoryCategories] cc
                    INNER JOIN CategoryTree ct ON cc.InstanceId = ct.InstanceId
                )
                SELECT DISTINCT p.* FROM [Instances].[Products] p
                INNER JOIN [Instances].[ProductCategories] pc ON p.InstanceId = pc.InstanceId
                WHERE pc.CategoryInstanceId IN (SELECT InstanceId FROM CategoryTree)");

            parameters.Add("CatIds", request.CategoryIds);

            if (request.Attributes != null && request.Attributes.Any())
            {
                for (int i = 0; i < request.Attributes.Count; i++)
                {
                    var attr = request.Attributes[i];
                    var keyParam = $"@key{i}";
                    var valParam = $"@val{i}";

                    sql.Append($@" 
                AND EXISTS (
                    SELECT 1 FROM [Instances].[ProductAttributes] pa 
                    WHERE pa.InstanceId = p.InstanceId 
                    AND pa.[Key] = {keyParam} 
                    AND pa.[Value] = {valParam}
                )");

                    parameters.Add(keyParam, attr.Key);
                    parameters.Add(valParam, attr.Value);
                }
            }
            
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
                    await conn.QueryAsync<Product>(sql.ToString(), parameters, trans));
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            const string sql = @"
                SELECT 
                    InstanceId, 
                    Name, 
                    Description, 
                    ProductImageUris, 
                    ValidSkus, 
                    CreatedTimestamp 
                FROM [Instances].[Products]
                ORDER BY CreatedTimestamp DESC;";

            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                return await conn.QueryAsync<Product>(sql, transaction: trans);
            });
        }
    }
}
