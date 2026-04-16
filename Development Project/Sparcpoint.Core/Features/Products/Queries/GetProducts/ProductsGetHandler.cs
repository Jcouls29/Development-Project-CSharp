using Dapper;
using MediatR;
using Sparcpoint.Models;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Features.Products.Queries.GetProducts
{
    public class ProductsGetHandler : IRequestHandler<ProductsGetQuery, List<ProductsGetResponseItem>>
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductsGetHandler(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        public async Task<List<ProductsGetResponseItem>> Handle(ProductsGetQuery request, CancellationToken cancellationToken)
        {
            return await _sqlExecutor.ExecuteAsync<List<ProductsGetResponseItem>>(async (connection, transaction) =>
            {
                var sql = new StringBuilder();
                var parameters = new DynamicParameters();

                sql.Append(@"
                    SELECT DISTINCT p.InstanceId, p.Name, p.Description, p.ProductImageUris, p.ValidSkus, p.CreatedTimestamp
                    FROM Instances.Products p");

                if (request.CategoryId.HasValue)
                {
                    sql.Append(@"
                    INNER JOIN Instances.ProductCategories pc ON pc.InstanceId = p.InstanceId");
                }

                if (request.Attributes != null && request.Attributes.Count > 0)
                {
                    for (int i = 0; i < request.Attributes.Count; i++)
                    {
                        sql.Append($@"
                    INNER JOIN Instances.ProductAttributes pa{i} ON pa{i}.InstanceId = p.InstanceId
                        AND pa{i}.[Key] = @AttrKey{i} AND pa{i}.[Value] = @AttrValue{i}");
                        parameters.Add($"AttrKey{i}", request.Attributes[i].Key);
                        parameters.Add($"AttrValue{i}", request.Attributes[i].Value);
                    }
                }

                var whereClauses = new List<string>();

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    whereClauses.Add("p.Name LIKE @Name");
                    parameters.Add("Name", $"%{request.Name}%");
                }

                if (!string.IsNullOrWhiteSpace(request.Description))
                {
                    whereClauses.Add("p.Description LIKE @Description");
                    parameters.Add("Description", $"%{request.Description}%");
                }

                if (request.CategoryId.HasValue)
                {
                    whereClauses.Add("pc.CategoryInstanceId = @CategoryId");
                    parameters.Add("CategoryId", request.CategoryId.Value);
                }

                if (whereClauses.Count > 0)
                {
                    sql.Append(" WHERE ");
                    sql.Append(string.Join(" AND ", whereClauses));
                }

                sql.Append(" ORDER BY p.Name");

                var products = (await connection.QueryAsync<ProductsGetResponseItem>(sql.ToString(), parameters, transaction)).ToList();

                if (products.Count == 0)
                    return products;

                var productIds = products.Select(p => p.InstanceId).ToList();

                var categories = (await connection.QueryAsync<ProductCategoryRow>(
                    @"SELECT pc.InstanceId AS ProductInstanceId, c.InstanceId, c.Name, c.Description
                      FROM Instances.ProductCategories pc
                      INNER JOIN Instances.Categories c ON c.InstanceId = pc.CategoryInstanceId
                      WHERE pc.InstanceId IN @Ids",
                    new { Ids = productIds },
                    transaction)).ToList();

                var attributes = (await connection.QueryAsync<ProductAttributeRow>(
                    @"SELECT InstanceId AS ProductInstanceId, [Key], [Value]
                      FROM Instances.ProductAttributes
                      WHERE InstanceId IN @Ids",
                    new { Ids = productIds },
                    transaction)).ToList();

                var categoryLookup = categories.ToLookup(c => c.ProductInstanceId);
                var attributeLookup = attributes.ToLookup(a => a.ProductInstanceId);

                foreach (var product in products)
                {
                    product.Categories = categoryLookup[product.InstanceId]
                        .Select(c => new ProductsGetCategoryItem
                        {
                            InstanceId = c.InstanceId,
                            Name = c.Name,
                            Description = c.Description
                        }).ToList();

                    product.Attributes = attributeLookup[product.InstanceId]
                        .Select(a => new AttributeItem
                        {
                            Key = a.Key,
                            Value = a.Value
                        }).ToList();
                }

                return products;
            });
        }

        private class ProductCategoryRow
        {
            public int ProductInstanceId { get; set; }
            public int InstanceId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

        private class ProductAttributeRow
        {
            public int ProductInstanceId { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}
