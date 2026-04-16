using Dapper;
using Newtonsoft.Json;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var parameters = new
            {
                product.Name,
                product.Description,
                ProductImageUris = JsonConvert.SerializeObject(product.ProductImageUris ?? new List<string>()),
                ValidSkus = JsonConvert.SerializeObject(product.ValidSkus ?? new List<string>())
            };

            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                int instanceId = await connection.QuerySingleAsync<int>(sqlProduct, parameters, transaction);

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

                product.Id = instanceId;
                return instanceId;
            });
        }

        public async Task<IEnumerable<Product>> SearchAsync(IEnumerable<int> categoryIds, string attrKey, string attrValue)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                // EVAL: The CTE "CategoryTree" finds all requested categories and their descendants.
                // Then we join with Products to get the final result filtered by Metadata.
                string sql = @"
            WITH CategoryTree AS (
                -- Base case: Categories passed as parameters
                SELECT InstanceId 
                FROM [Instances].[Categories] 
                WHERE InstanceId IN @CatIds
                
                UNION ALL
                
                -- Find children of the found categories
                SELECT cc.CategoryInstanceId
                FROM [Instances].[CategoryCategories] cc
                INNER JOIN CategoryTree ct ON cc.InstanceId = ct.InstanceId
            )
            SELECT DISTINCT p.* FROM [Instances].[Products] p
            INNER JOIN [Instances].[ProductCategories] pc ON p.InstanceId = pc.InstanceId
            ";

                var parameters = new DynamicParameters();
                parameters.Add("CatIds", categoryIds);

                if (!string.IsNullOrWhiteSpace(attrKey))
                {
                    sql += " INNER JOIN [Instances].[ProductAttributes] pa ON p.InstanceId = pa.InstanceId ";
                }

                // Filters
                List<string> filters = new List<string>();

                if (categoryIds != null && categoryIds.Any())
                    filters.Add("pc.CategoryInstanceId IN (SELECT InstanceId FROM CategoryTree)");

                if (!string.IsNullOrWhiteSpace(attrKey))
                {
                    filters.Add("pa.[Key] = @Key");
                    filters.Add("pa.[Value] = @Value");
                    parameters.Add("Key", attrKey);
                    parameters.Add("Value", attrValue);
                }

                if (filters.Any())
                    sql += " WHERE " + string.Join(" AND ", filters);

                return await connection.QueryAsync<Product>(sql, parameters, transaction);
            });
        }
    }
}
