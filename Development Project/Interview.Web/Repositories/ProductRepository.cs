using Dapper;
using Interview.Web.Contracts;
using Sparcpoint.Contracts;
using Sparcpoint.Domain;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations.Repositories
{
    internal class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor sql;
        private readonly IQueryInterpreter<Product> queryInterpreter;

        public ProductRepository(ISqlExecutor sql,IQueryInterpreter<Product> queryInterpreter)
        {
            this.sql = sql;
            this.queryInterpreter = queryInterpreter;
        }
        public async Task<int> Add(Product product)
        {
            var productCreated = await sql.ExecuteAsync(async (conn, tran) =>
            {
                var instanceId = 0;
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO Instances.Products (Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp) VALUES (@Name, @Description, @ProductImageUris, @ValidSkus, @CreatedTimestamp);SELECT SCOPE_IDENTITY()";
                    command.Transaction = tran;

                    var nameParameter = command.CreateParameter();
                    nameParameter.ParameterName = "Name";
                    nameParameter.Value = product.Name;
                    command.Parameters.Add(nameParameter);

                    var descParameter = command.CreateParameter();
                    descParameter.ParameterName = "Description";
                    descParameter.Value = product.Description;
                    command.Parameters.Add(descParameter);

                    var productImagesParameter = command.CreateParameter();
                    productImagesParameter.ParameterName = "ProductImageUris";
                    productImagesParameter.Value = product.ProductImageUris;
                    command.Parameters.Add(productImagesParameter);

                    var validSkuParameter = command.CreateParameter();
                    validSkuParameter.ParameterName = "ValidSkus";
                    validSkuParameter.Value = product.ValidSkus;
                    command.Parameters.Add(validSkuParameter);

                    var timeStampParameter = command.CreateParameter();
                    timeStampParameter.ParameterName = "CreatedTimestamp";
                    timeStampParameter.Value = product.CreatedTimestamp;
                    command.Parameters.Add(timeStampParameter);

                    object result = command.ExecuteScalar();
                    product.InstanceId = Convert.ToInt32(result);
                }

                foreach (var attribute in product.Attributes)
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = @"INSERT INTO Instances.ProductAttributes (InstanceId, [Key], [Value]) VALUES (@ProductInstanceId, @Key, @Value);";
                        command.Transaction = tran;

                        var productInstanceIdParameter = command.CreateParameter();
                        productInstanceIdParameter.ParameterName = "ProductInstanceId";
                        productInstanceIdParameter.Value = product.InstanceId;
                        command.Parameters.Add(productInstanceIdParameter);

                        var keyParameter = command.CreateParameter();
                        keyParameter.ParameterName = "Key";
                        keyParameter.Value = attribute.Key;
                        command.Parameters.Add(keyParameter);

                        var valueParameter = command.CreateParameter();
                        valueParameter.ParameterName = "Value";
                        valueParameter.Value = attribute.Value;
                        command.Parameters.Add(valueParameter);

                        command.ExecuteNonQuery();
                    }
                }
                return product.InstanceId;
            });

            return productCreated;
        }

        public async Task<IEnumerable<Product>> Get(string filter)
        {
            return await sql.ExecuteAsync<IEnumerable<Product>>(async (conn, trans) =>
            {
                List<Product> products = new List<Product>();
                using (var command = conn.CreateCommand())
                {
                    string query = @"SELECT p.InstanceId, p.Name, p.Description, p.ProductImageUris, p.ValidSkus FROM [Instances].[Products] p ";
                    var queryProvider = queryInterpreter.Interpret(filter);
                    query += queryProvider.WhereClause;
                    command.CommandText = query;
                    command.Transaction = trans;
                    var result = command.ExecuteReader();
                    while (result.Read())
                    {
                        var product = new Product
                        {
                            InstanceId = result.GetInt32(0),
                            Name = result.GetString(1),
                            Description = result.GetString(2),
                            ProductImageUris = result.GetString(3),
                            ValidSkus = result.GetString(4)
                        };
                        products.Add(product);
                    }
                }
                return products;
            });
        }
    }
}
