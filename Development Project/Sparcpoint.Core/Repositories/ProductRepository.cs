using Dapper;
using Newtonsoft.Json;
using Sparcpoint.Abstract;
using Sparcpoint.DTO;
using Sparcpoint.Models;
using Sparcpoint.Requests;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductRepository(ISqlExecutor executor)
        {
            _sqlExecutor = executor;
        }
        public async Task<int> CreateProductAsync(CreateProductRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));
            PreConditions.StringNotNullOrWhitespace(request.Name, nameof(request.Name));

            //EVAL: using dapper queries for readability and simplicity.
            //This could be optimized with stored procedures or more complex queries if needed,
            //but for this example it is straight forward to understand and maintain with dapper.

            try
            {
                return await _sqlExecutor.ExecuteAsync((conn, trans) =>
                    CreateProductInternalAsync(conn, trans, request));
            }
            catch (Exception ex)
            {
                //EVAL: normally this exception would be logged and then returned back through the API (not implemented here)
                throw new Exception("Error creating product.", ex);
            }            
        }

        //EVAL: this method is used to encapsulate the entire product creation process within a single transaction scope,
        //ensuring that all related inserts (product, attributes, categories) either succeed or fail together for data integrity.
        private async Task<int> CreateProductInternalAsync(
                                        IDbConnection conn,
                                        IDbTransaction trans,
                                        CreateProductRequest request)
        {
            var newInstanceId = await InsertProductAsync(conn, trans, request);

            if (request.Attributes?.Any() == true)
            {
                await InsertAttributesAsync(conn, trans, newInstanceId, request.Attributes);
            }

            if (request.CategoryInstanceIds?.Any() == true)
            {
                await InsertCategoriesAsync(conn, trans, newInstanceId, request.CategoryInstanceIds);
            }

            return newInstanceId;
        }

        private async Task<int> InsertProductAsync(
                                    IDbConnection conn,
                                    IDbTransaction trans,
                                    CreateProductRequest request)
        {
            const string sql = @"
                    INSERT INTO [Instances].[Products]
                        (Name, Description, ProductImageUris, ValidSkus)
                    OUTPUT INSERTED.InstanceId
                    VALUES
                        (@Name, @Description, @ProductImageUris, @ValidSkus);";

            return await conn.ExecuteScalarAsync<int>(sql, new
            {
                request.Name,
                request.Description,
                ProductImageUris = JsonConvert.SerializeObject(request.ProductImageUris ?? new List<string>()),
                ValidSkus = JsonConvert.SerializeObject(request.ValidSkus ?? new List<string>())
            }, trans);
        }

        private async Task InsertAttributesAsync(
                                IDbConnection conn,
                                IDbTransaction trans,
                                int instanceId,
                                IDictionary<string, string> attributes)
        {
            const string sql = @"
                INSERT INTO [Instances].[ProductAttributes]
                    (InstanceId, [Key], Value)
                VALUES
                    (@InstanceId, @Key, @Value);";

            var rows = attributes.Select(kvp => new
            {
                InstanceId = instanceId,
                kvp.Key,
                kvp.Value
            });

            await conn.ExecuteAsync(sql, rows, trans);
        }

        private async Task InsertCategoriesAsync(
                                IDbConnection conn,
                                IDbTransaction trans,
                                int instanceId,
                                IEnumerable<int> categoryIds)
        {
            const string sql = @"
                INSERT INTO [Instances].[ProductCategories]
                    (InstanceId, CategoryInstanceId)
                VALUES
                    (@InstanceId, @CategoryInstanceId);";

            var rows = categoryIds.Select(id => new
            {
                InstanceId = instanceId,
                CategoryInstanceId = id
            });

            await conn.ExecuteAsync(sql, rows, trans);
        }

        public async Task<IEnumerable<Product>> SearchProductAsync(SearchProductRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));
            try
            {
                return await _sqlExecutor.ExecuteAsync((conn, trans) =>
                    SearchProductsInternalAsync(conn, trans, request));
            }
            catch (Exception ex)
            {
                //EVAL: normally this exception would be logged and then returned back through the API (not implemented here)
                throw new Exception("Error searching products.", ex);
            }
        }

        private async Task<IEnumerable<Product>> SearchProductsInternalAsync(
                                                            IDbConnection conn,
                                                            IDbTransaction trans,
                                                            SearchProductRequest request)
        {
            var sqlBuilder = new StringBuilder();
            var parameters = new DynamicParameters();
            //EVAL: I Played with using common table expression (CTE) to filter products based on the
            //search criteria (name, categories, attributes) and then joining back to the main tables to
            //retrieve all relevant data in a single query with multiple result sets for better performance.
            //There were some scope and syntax issues with the CTE approach, so I switched to using a temporary
            //table to store the filtered product instance ids first, and then join back to the main tables to
            //retrieve the data.            
            sqlBuilder.Append(@"
            CREATE TABLE #FilteredProducts (InstanceId INT PRIMARY KEY);

            INSERT INTO #FilteredProducts (InstanceId)
            SELECT DISTINCT p.InstanceId
            FROM [Instances].[Products] p
            LEFT JOIN [Instances].[ProductCategories] pc ON p.InstanceId = pc.InstanceId
            LEFT JOIN [Instances].[ProductAttributes] pa ON p.InstanceId = pa.InstanceId
            WHERE 1 = 1");

            // Name search
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                sqlBuilder.Append(" AND (p.Name LIKE @Search OR p.Description LIKE @Search)");
                parameters.Add("@Search", $"%{request.Name}%");
            }

            // Category filter
            if (request.CategoryInstanceIds?.Any() == true)
            {
                sqlBuilder.Append(" AND pc.CategoryInstanceId IN @CategoryIds");
                parameters.Add("@CategoryIds", request.CategoryInstanceIds);
            }

            // Attribute filters (key/value pairs)
            if (request.Attributes?.Any() == true)
            {
                int i = 0;
                foreach (var kvp in request.Attributes)
                {
                    sqlBuilder.Append($@"
                AND EXISTS (
                    SELECT 1 FROM [Instances].[ProductAttributes] pa{i}
                    WHERE pa{i}.InstanceId = p.InstanceId
                    AND pa{i}.[Key] = @Key{i}
                    AND pa{i}.[Value] = @Value{i}
                )");

                    parameters.Add($"@Key{i}", kvp.Key);
                    parameters.Add($"@Value{i}", kvp.Value);
                    i++;
                }
            }            

            // Final QueryMultiple (filtered)
            sqlBuilder.Append(@"

               -- Products
                SELECT p.InstanceId, p.Name, p.Description, p.ProductImageUris, p.ValidSkus, p.CreatedTimestamp
                FROM [Instances].[Products] p
                INNER JOIN #FilteredProducts fp ON p.InstanceId = fp.InstanceId;

                -- Attributes
                SELECT pa.InstanceId, pa.[Key], pa.[Value]
                FROM [Instances].[ProductAttributes] pa
                INNER JOIN #FilteredProducts fp ON pa.InstanceId = fp.InstanceId;

                -- Categories
                SELECT pc.InstanceId, pc.CategoryInstanceId
                FROM [Instances].[ProductCategories] pc
                INNER JOIN #FilteredProducts fp ON pc.InstanceId = fp.InstanceId;

                DROP TABLE #FilteredProducts;
            ");

            using(var multi = await conn.QueryMultipleAsync(sqlBuilder.ToString(), parameters, trans))
            {
                var products = (await multi.ReadAsync<ProductDto>()).ToList();
                var attributes = (await multi.ReadAsync<ProductAttributeDto>()).ToList();
                var categories = (await multi.ReadAsync<ProductCategoryDto>()).ToList();
                return MapToProducts(products, attributes, categories);
            }            
        }
        //EVAL: this method using querymultiple to retrieve all products, their attributes, and categories in one round trip to the database for better performance.
        private async Task<IEnumerable<Product>> ExecuteGetAllWithQueryMultipleAsync(IDbConnection conn, IDbTransaction trans)
        {
            const string sql = @"
                -- 1. Products
                SELECT InstanceId, Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp
                FROM [Instances].[Products]
                ORDER BY CreatedTimestamp DESC;

                -- 2. Attributes
                SELECT InstanceId, [Key], [Value]
                FROM [Instances].[ProductAttributes];

                -- 3. Categories
                SELECT InstanceId, CategoryInstanceId
                FROM [Instances].[ProductCategories];
    ";

            using (var multi = await conn.QueryMultipleAsync(sql, transaction: trans))
            {
                // Read all 3 result sets
                var products = (await multi.ReadAsync<ProductDto>()).ToList();
                var attributes = (await multi.ReadAsync<ProductAttributeDto>()).ToList();
                var categories = (await multi.ReadAsync<ProductCategoryDto>()).ToList();

                // Map to domain
                var result = MapToProducts(products, attributes, categories);

                return result;
            }            
        }

        private IEnumerable<Product> MapToProducts(
                        List<ProductDto> productDtos,
                        List<ProductAttributeDto> attributeDtos,
                        List<ProductCategoryDto> categoryDtos)
        {
            var attributeLookup = attributeDtos
                .GroupBy(a => a.InstanceId)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(a => a.Key, a => a.Value)
                );

            var categoryLookup = categoryDtos
                .GroupBy(c => c.InstanceId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(c => c.CategoryInstanceId).ToList()
                );

            var products = new List<Product>();

            foreach (var dto in productDtos)
            {
                var product = new Product
                {
                    Id = dto.InstanceId,
                    Name = dto.Name,
                    Description = dto.Description,
                    CreatedTimestamp = dto.CreatedTimestamp,
                    ProductImageUris = dto.ProductImageUris,
                    ValidSkus = dto.ValidSkus,
                    Attributes = attributeLookup.TryGetValue(dto.InstanceId, out var attrs)
                        ? attrs
                        : new Dictionary<string, string>(),
                    CategoryInstanceIds = categoryLookup.TryGetValue(dto.InstanceId, out var cats)
                        ? cats
                        : new List<int>()
                };                
                products.Add(product);
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            try
            {
                //EVAL: this method uses a single query with multiple result sets to retrieve all products,
                //their attributes, and categories in one round trip to the database for better performance.               
                return await _sqlExecutor.ExecuteAsync(ExecuteGetAllWithQueryMultipleAsync);
                             
            }
            catch (Exception ex)
            {
                //EVAL: normally this exception would be logged and then returned back through the API (not implemented here)
                //multiple catches could be implemented to handle each SQL transaction separately, but for simplicity
                //this will just catch any exception that occurs during the entire process,
                //and the status code returned to the API would be a generic 500 error caught with the global exception handler with a
                //message indicating that an error occurred while retrieving products.                
                throw new Exception("An error occurred while retrieving products.", ex);
            }            
        }
    }
}
