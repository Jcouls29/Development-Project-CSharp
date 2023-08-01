using Dapper;
using Interview.Web.Models;
using System.Data;
using System;
using System.Threading.Tasks;
using Interview.Web.Dtos;

namespace Interview.Web.Data
{
    public class InventoryRepo : IInventoryRepo
    {
        private readonly DataContext _dataContext;

        public InventoryRepo(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Product> CreateProduct(ProductCreateDto product)
        {
            var query = "INSERT INTO Instances.Products (Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp) VALUES (@Name, @Description, @ProductImageUris, @ValidSkus, @CreatedTimestamp)"
                        + "SELECT CAST(SCOPE_IDENTITY() as int)";

            var parameters = new DynamicParameters();
            parameters.Add("Name", product.Name, DbType.String);
            parameters.Add("Description", product.Description, DbType.String);
            parameters.Add("ProductImageUris", product.ProductImageUris, DbType.String);
            parameters.Add("ValidSkus", product.ValidSkus, DbType.String);

            var createdTimeStamp = DateTime.Now;

            parameters.Add("CreatedTimestamp", createdTimeStamp, DbType.DateTime);

            using (var connection = _dataContext.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);

                var createdProduct = new Product
                {
                    InstanceId = id,
                    Name = product.Name,
                    Description = product.Description,
                    ProductImageUris = product.ProductImageUris,
                    ValidSkus = product.ValidSkus,
                    CreatedTimeStamp = createdTimeStamp,
                    Attributes  = product.Attributes
                };

                if (product.Categories.Count > 0)
                {
                    foreach (var category in product.Categories)
                    {
                        var categoryQuery = "INSERT INTO Instances.Categories (Name, Description, CreatedTimestamp) VALUES (@Name, @Description, @CreatedTimestamp)"
                                            + "SELECT CAST(SCOPE_IDENTITY() as int)";

                        var categoryParameters = new DynamicParameters();
                        categoryParameters.Add("Name", category.Name, DbType.String);
                        categoryParameters.Add("Description", category.Description, DbType.String);
                        categoryParameters.Add("CreatedTimestamp", createdTimeStamp, DbType.DateTime);

                        var categoryId = await connection.QuerySingleAsync<int>(categoryQuery, categoryParameters);

                        var createdCategory = new Category
                        {
                            InstanceId = categoryId,
                            Name = category.Name,
                            Description = category.Description,
                            CreatedTimeStamp = createdTimeStamp
                        };

                        createdProduct.Categories.Add(createdCategory);

                        await SetProductCategories(createdProduct.InstanceId, createdCategory.InstanceId);
                    }
                }

                if(product.Attributes.Count > 0)
                {
                    foreach (var attribute in product.Attributes)
                    {
                        await SetProductAttributes(createdProduct.InstanceId, attribute);
                    }
                }

                return createdProduct;
            }

        }

        public async Task<Product> GetProductById(int id)
        {
            var query = "SELECT * FROM Instances.Products WHERE InstanceId = @Id";

            using (var connection = _dataContext.CreateConnection())
            {
                var product = await connection.QuerySingleOrDefaultAsync<Product>(query, new { id });
                return product;
            }
        }

        //Set Product Categories in JOIN Tabnle
        private async Task SetProductCategories(int productId, int categoryId)
        {
            var query = "INSERT INTO Instances.ProductCategories (InstanceId, CategoryInstanceId) VALUES (@InstanceId, @CategoryInstanceId)";

            var parameters = new DynamicParameters();
            parameters.Add("InstanceId", productId, DbType.Int64);
            parameters.Add("CategoryInstanceId", categoryId, DbType.Int64);

            using (var connection = _dataContext.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        //Set Product Attributes in JOIN Table
        private async Task SetProductAttributes(int instanceId, string attribute)
        {
            var query = "INSERT INTO Instances.ProductAttributes (InstanceId, [Key], Value) VALUES (@InstanceId, @Key, @Value)";

            var parameters = new DynamicParameters();
            parameters.Add("InstanceId", instanceId, DbType.Int64);
            parameters.Add("Key", attribute, DbType.String);
            parameters.Add("Value", attribute, DbType.String);

            using (var connection = _dataContext.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
