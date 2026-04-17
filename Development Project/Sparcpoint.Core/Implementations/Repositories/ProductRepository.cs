using Dapper;
using Sparcpoint.Abstract.Repositories;
using Sparcpoint.Domain;
using Sparcpoint.DTOs;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _executor;

        public ProductRepository(ISqlExecutor executor)
        {
            _executor = executor;
        }

        public async Task<int> AddProductAsync(CreateProductRequestDto request)
        {
            return await _executor.ExecuteAsync<int>(async (connection, transaction) =>
            {
                string prodSql = @"
                    INSERT INTO [Instances].[Products] (Name, Description, ProductImageUris, ValidSkus)
                    VALUES (@Name, @Description, @ImageUris, @Skus);
                    SELECT SCOPE_IDENTITY();";

                var productId = Convert.ToInt32(await connection.ExecuteScalarAsync(prodSql, new
                {
                    request.Name,
                    request.Description,
                    request.ImageUris,
                    request.Skus
                }, transaction));

                if (request.Attributes != null && request.Attributes.Any())
                {
                    string attrSql = "INSERT INTO [Instances].[ProductAttributes] (InstanceId, [Key], [Value]) VALUES (@InstanceId, @Key, @Value)";
                    foreach (var attr in request.Attributes)
                    {
                        await connection.ExecuteAsync(attrSql, new { InstanceId = productId, attr.Key, attr.Value }, transaction);
                    }
                }

                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    string catSql = "INSERT INTO [Instances].[ProductCategories] (InstanceId, CategoryId) VALUES (@InstanceId, @CategoryId)";
                    foreach (var catId in request.CategoryIds)
                    {
                        await connection.ExecuteAsync(catSql, new { InstanceId = productId, CategoryId = catId }, transaction);
                    }
                }

                return productId;
            });
        }


        public async Task<IEnumerable<Product>> SearchProductsAsync(ProductSearchRequestDto request)
        {
            return await _executor.ExecuteAsync<IEnumerable<Product>>(async (connection, transaction) =>
            {
                var queryProvider = new SqlServerQueryProvider().SetTargetTableAlias("p");

                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    string paramName = queryProvider.GetNextParameterName("@Search");
                    queryProvider.Where($"(p.[Name] LIKE {paramName} OR p.[Description] LIKE {paramName})");
                    queryProvider.AddParameter(paramName, $"%{request.SearchText}%");
                }

                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    queryProvider.Join("[Instances].[ProductCategories]", "pc", "p.InstanceId = pc.InstanceId");
                    queryProvider.WhereIn("pc.CategoryInstanceId", "@CategoryIds");
                    queryProvider.AddParameter("@CategoryIds", request.CategoryIds);
                }

                string attributeFilter = "";
                if (request.Attributes != null && request.Attributes.Any())
                {
                    queryProvider.Join("[Instances].[ProductAttributes]", "pa", "p.InstanceId = pa.InstanceId");

                    // EVAL: create de key,value sentence
                    var attrConditions = new List<string>();
                    for (int i = 0; i < request.Attributes.Count; i++)
                    {
                        string keyParam = $"@attributeKey{i}";
                        string valueParam = $"@attributeValue{i}";

                        attrConditions.Add($"(pa.[Key] = {keyParam} AND pa.[Value] = {valueParam})");

                        queryProvider.AddParameter(keyParam, request.Attributes[i].Key);
                        queryProvider.AddParameter(valueParam, request.Attributes[i].Value);
                    }

                    // EVAL: Create attribute filter
                    queryProvider.Where("(" + string.Join(" OR ", attrConditions) + ")");

                    // EVAL: Get products with all those attributes.
                    attributeFilter = $"GROUP BY p.[InstanceId], p.[Name], p.[Description], p.[ProductImageUris], p.[ValidSkus], p.[CreatedTimestamp] " +
                                       $"HAVING COUNT(DISTINCT pa.[Key]) = {request.Attributes.Count}";
                }

                string sql = $@"
                    SELECT p.[InstanceId], 
                            p.[Name], 
                            p.[Description], 
                            p.[ProductImageUris], 
                            p.[ValidSkus], 
                            p.[CreatedTimestamp]
                    FROM [Instances].[Products] p
                    {queryProvider.JoinClause}
                    {queryProvider.WhereClause}
                    {attributeFilter}
                    ORDER BY p.CreatedTimestamp DESC";

                return await connection.QueryAsync<Product>(sql, queryProvider.Parameters, transaction);
            });

        }
    }
}
