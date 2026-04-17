using Dapper;
using Sparcpoint.Abstract.Repositories;
using Sparcpoint.Models;
using Sparcpoint.Request;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Task<int> CreateAsync(CreateProductRequest createProductRequest)
        {
            var json = new JsonDataSerializer();
            return _sqlExecutor.ExecuteAsync(async (db, tx) =>
            {
                const string insertProductQuery = @"
                    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus])
                    OUTPUT INSERTED.[InstanceId]
                    VALUES (@ProductName, @ProductDesc, @Images, @Skus);";

                var productParameters = new
                {
                    ProductName = createProductRequest.Name,
                    ProductDesc = createProductRequest.Description,
                    Images = json.Serialize(createProductRequest.ProductImageUris),
                    Skus = json.Serialize(createProductRequest.ValidSkus)
                };

                int newProductId = await db.ExecuteScalarAsync<int>(insertProductQuery, productParameters, tx);

                if (createProductRequest.Attributes != null && createProductRequest.Attributes.Count > 0)
                {
                    const string insertAttributeQuery = @"
                        INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
                        VALUES (@Id, @AttrKey, @AttrValue);";

                    foreach (var kvp in createProductRequest.Attributes)
                    {
                        await db.ExecuteAsync(insertAttributeQuery, new
                        {
                            Id = newProductId,
                            AttrKey = kvp.Key,
                            AttrValue = kvp.Value
                        }, tx);
                    }
                }

                if (createProductRequest.CategoryIds != null && createProductRequest.CategoryIds.Count > 0)
                {
                    const string insertCategoryQuery = @"
                        INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
                        VALUES (@Id, @CatId);";

                    foreach (var catId in createProductRequest.CategoryIds)
                    {
                        await db.ExecuteAsync(insertCategoryQuery, new
                        {
                            Id = newProductId,
                            CatId = catId
                        }, tx);
                    }
                }

                return newProductId;
            });
        }

        public Task<ProductDetail> GetByIdAsync(int instanceId)
        {
            return _sqlExecutor.ExecuteAsync(async (db, tx) =>
            {
                const string sqlQuery = @"
                    SELECT p.[InstanceId], p.[Name], p.[Description], p.[ProductImageUris], p.[ValidSkus], p.[CreatedTimestamp]
                    FROM [Instances].[Products] p
                    WHERE p.[InstanceId] = @Id;

                    SELECT pa.[Key], pa.[Value]
                    FROM [Instances].[ProductAttributes] pa
                    WHERE pa.[InstanceId] = @Id;

                    SELECT c.[InstanceId], c.[Name], c.[Description], c.[CreatedTimestamp]
                    FROM [Instances].[ProductCategories] pc
                    INNER JOIN [Instances].[Categories] c ON c.[InstanceId] = pc.[CategoryInstanceId]
                    WHERE pc.[InstanceId] = @Id
                    ORDER BY c.[Name] ASC;";

                using (var multiReader = await db.QueryMultipleAsync(sqlQuery, new { Id = instanceId }, tx))
                {
                    var productData = await multiReader.ReadSingleOrDefaultAsync<Product>();

                    if (productData == null)
                    {
                        return null;
                    }

                    var attrDict = (await multiReader.ReadAsync<ProductAttribute>()).ToDictionary(x => x.Key, x => x.Value);

                    var categoryList = (await multiReader.ReadAsync<ProductCategory>()).ToList();

                    var result = new ProductDetail
                    {
                        InstanceId = productData.InstanceId,
                        Name = productData.Name,
                        Description = productData.Description,
                        CreatedTimestamp = productData.CreatedTimestamp,
                        ProductImageUris = productData.ProductImageUris,
                        ValidSkus = productData.ValidSkus,
                        Attributes = attrDict,
                        Categories = categoryList
                    };

                    return result;
                }
            });
        }

        public Task<List<ProductDetail>> SearchAsync(ProductSearchRequest productSearchrequest)
        {
            return _sqlExecutor.ExecuteAsync(async (db, tx) =>
            {
                var builder = new SqlServerQueryProvider()
                    .SetTargetTableAlias("p")
                    .OrderByAscending("CreatedTimestamp");

                if (!string.IsNullOrEmpty(productSearchrequest.Name))
                {
                    var pName = builder.GetNextParameterName("@Name");
                    builder.Where($"p.[Name] LIKE {pName}");
                    builder.AddParameter(pName, $"%{productSearchrequest.Name}%");
                }

                if (!string.IsNullOrEmpty(productSearchrequest.Description))
                {
                    var pDesc = builder.GetNextParameterName("@Description");
                    builder.Where($"p.[Description] LIKE {pDesc}");
                    builder.AddParameter(pDesc, $"%{productSearchrequest.Description}%");
                }

                foreach (var attribute in productSearchrequest.Attributes)
                {
                    var keyParameter = builder.GetNextParameterName("@AttributeKey");
                    var valueParameter = builder.GetNextParameterName("@AttributeValue");

                    builder.Where($@"EXISTS (
                        SELECT 1
                        FROM [Instances].[ProductAttributes] pa
                        WHERE pa.[InstanceId] = p.[InstanceId]
                          AND pa.[Key] = {keyParameter}
                          AND pa.[Value] LIKE {valueParameter}
                    )");

                    builder.AddParameter(keyParameter, attribute.Key);
                    builder.AddParameter(valueParameter, $"%{attribute.Value}%");
                }

                var parameterNames = new List<string>();
                foreach (var categoryId in productSearchrequest.CategoryIds.Distinct())
                {
                    var parameterName = builder.GetNextParameterName("@CategoryId");
                    builder.AddParameter(parameterName, categoryId);
                    parameterNames.Add(parameterName);
                }

                builder.Where($@"EXISTS (
                    SELECT 1
                    FROM [Instances].[ProductCategories] pc
                    WHERE pc.[InstanceId] = p.[InstanceId]
                      AND pc.[CategoryInstanceId] IN ({string.Join(", ", parameterNames)})
                )");

                var queryBuilder = new StringBuilder();

                queryBuilder.AppendLine("SELECT COUNT(*) FROM [Instances].[Products] p");
                if (!string.IsNullOrWhiteSpace(builder.JoinClause)) queryBuilder.AppendLine(builder.JoinClause);
                queryBuilder.AppendLine(!string.IsNullOrWhiteSpace(builder.WhereClause) ? $"{builder.WhereClause};" : ";");

                queryBuilder.AppendLine();

                queryBuilder.AppendLine("SELECT p.[InstanceId], p.[Name], p.[Description], p.[ProductImageUris], p.[ValidSkus], p.[CreatedTimestamp]");
                queryBuilder.AppendLine("FROM [Instances].[Products] p");

                if (!string.IsNullOrWhiteSpace(builder.JoinClause)) queryBuilder.AppendLine(builder.JoinClause);
                if (!string.IsNullOrWhiteSpace(builder.WhereClause)) queryBuilder.AppendLine(builder.WhereClause);

                queryBuilder.AppendLine(builder.OrderByClause);
                queryBuilder.AppendLine("OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;");

                var queryParams = new DynamicParameters();
                foreach (var p in builder.Parameters)
                    queryParams.Add(p.Key, p.Value);
                                              

                using (var multi = await db.QueryMultipleAsync(queryBuilder.ToString(), queryParams, tx))
                {
                    var pRows = await multi.ReadAsync<Product>();
                    return  pRows.Select(p => new ProductDetail
                    {
                        InstanceId = p.InstanceId,
                        Name = p.Name,
                        Description = p.Description,
                        CreatedTimestamp = p.CreatedTimestamp,
                        ProductImageUris = p.ProductImageUris,
                        ValidSkus = p.ValidSkus,                
                        Attributes = new Dictionary<string, string>(),
                        Categories = new List<ProductCategory>()
                    }).ToList();                    
                }
            });
        }
    }
}
