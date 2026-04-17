using Sparcpoint.Interfaces;
using Sparcpoint.Models;
using Sparcpoint.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using Sparcpoint.SqlServer.Abstractions;


namespace Sparcpoint.Implementations.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _executor;

        public ProductRepository(ISqlExecutor executor)
        {
            _executor = executor;
        }

        // Crea producto y categorías en una sola operación
        public async Task<int> CreateAsync(CreateProductRequest request)
        {
            return await _executor.ExecuteAsync<int>(async (conn, trans) =>
            {
                // Insertar producto
                var sql = @"
                    INSERT INTO [Instances].[Products] 
                        (Name, Description, ProductImageUris, ValidSkus)
                    VALUES 
                        (@Name, @Description, @ProductImageUris, @ValidSkus);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var instanceId = await conn.QuerySingleAsync<int>(sql, new
                {
                    request.Name,
                    request.Description,
                    request.ProductImageUris,
                    request.ValidSkus
                }, trans);

                // Insertar atributos (metadata)
                if (request.Attributes != null && request.Attributes.Count > 0)
                {
                    var attrSql = @"
                        INSERT INTO [Instances].[ProductAttributes] (InstanceId, [Key], Value)
                        VALUES (@InstanceId, @Key, @Value);";

                    foreach (var attr in request.Attributes)
                    {
                        await conn.ExecuteAsync(attrSql, new
                        {
                            InstanceId = instanceId,
                            Key = attr.Key,
                            Value = attr.Value
                        }, trans);
                    }
                }

                // Insertar categorías
                if (request.CategoryIds != null && request.CategoryIds.Count > 0)
                {
                    var catSql = @"
                        INSERT INTO [Instances].[ProductCategories] (InstanceId, CategoryId)
                        VALUES (@InstanceId, @CategoryId);";

                    foreach (var categoryId in request.CategoryIds)
                    {
                        await conn.ExecuteAsync(catSql, new
                        {
                            InstanceId = instanceId,
                            CategoryId = categoryId
                        }, trans);
                    }
                }

                return instanceId;
            });
        }

        public async Task<Product> GetByIdAsync(int instanceId)
        {
            return await _executor.ExecuteAsync<Product>(async (conn, trans) =>
            {
                // Obtener producto
                var sql = @"
                    SELECT * FROM [Instances].[Products] 
                    WHERE InstanceId = @InstanceId";

                var product = await conn.QuerySingleOrDefaultAsync<Product>(
                    sql, new { InstanceId = instanceId }, trans);

                if (product == null) return null;

                // Obtener atributos
                var attrSql = @"
                    SELECT * FROM [Instances].[ProductAttributes] 
                    WHERE InstanceId = @InstanceId";

                var attributes = await conn.QueryAsync<ProductAttribute>(
                    attrSql, new { InstanceId = instanceId }, trans);

                product.Attributes = attributes.ToList();

                return product;
            });
        }

        // Búsqueda por nombre, categorías y/o metadata
        public async Task<IEnumerable<Product>> SearchAsync(ProductSearchRequest request)
        {
            return await _executor.ExecuteAsync<IEnumerable<Product>>(async (conn, trans) =>
            {
                var sql = @"
                    SELECT DISTINCT p.* 
                    FROM [Instances].[Products] p";

                // Join condicional por categorías
                if (request.CategoryIds != null && request.CategoryIds.Count > 0)
                    sql += @" INNER JOIN [Instances].[ProductCategories] pc 
                              ON p.InstanceId = pc.InstanceId";

                // Join condicional por atributos
                if (request.Attributes != null && request.Attributes.Count > 0)
                    sql += @" INNER JOIN [Instances].[ProductAttributes] pa 
                              ON p.InstanceId = pa.InstanceId";

                sql += " WHERE 1=1";

                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(request.Name))
                {
                    sql += " AND p.Name LIKE @Name";
                    parameters.Add("Name", $"%{request.Name}%");
                }

                if (request.CategoryIds != null && request.CategoryIds.Count > 0)
                {
                    sql += " AND pc.CategoryId IN @CategoryIds";
                    parameters.Add("CategoryIds", request.CategoryIds);
                }

                // Búsqueda por múltiples pares key/value de metadata
                if (request.Attributes != null && request.Attributes.Count > 0)
                {
                    int i = 0;
                    foreach (var attr in request.Attributes)
                    {
                        sql += $" AND EXISTS (SELECT 1 FROM [Instances].[ProductAttributes] " +
                               $"WHERE InstanceId = p.InstanceId " +
                               $"AND [Key] = @AttrKey{i} AND Value = @AttrVal{i})";
                        parameters.Add($"AttrKey{i}", attr.Key);
                        parameters.Add($"AttrVal{i}", attr.Value);
                        i++;
                    }
                }

                var products = await conn.QueryAsync<Product>(sql, parameters, trans);
                return products.ToList();
            });
        }
    }
}
