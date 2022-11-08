using Dapper;
using Sparcpoint.Abstract.Products;
using Sparcpoint.Models.Products;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations.Products
{
    public class BaseProductRepository : IBaseProductRepository
    {
        private readonly ISqlExecutor _SqlExecutor;

        public BaseProductRepository(ISqlExecutor sqlExecutor)
        {
            _SqlExecutor = sqlExecutor;
        }

        public async Task<int> CreateAsync(BaseProduct product)
        {
            try
            {
                //Eval : Stored procedure can also be used here instead of direct sql statments
                string insertProductQuery = @"INSERT INTO [Instances].[Products] 
                (Name, Description, ProductImageUris, ValidSkus)
                VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                SELECT SCOPE_IDENTITY();";

                string insertProductMetadataQuery = $@"INSERT INTO [Instances].[ProductAttributes] 
                ([InstanceId], [Key], [Value])
                VALUES (@InstanceId, @Key, @Value);";

                return await _SqlExecutor.ExecuteAsync<int>(async (sqlConnection, sqlTransaction) =>
                {
                    int productId = await sqlConnection.QuerySingleAsync<int>(insertProductQuery, new
                    {
                        product,
                        product.Description,
                        product.ProductImageUris,
                        product.ValidSkus,
                    }, sqlTransaction);

                    // Add product metadata and pass product id created from above insert
                    if (product.Metadata != null && product.Metadata.Count >0)
                    {
                        foreach (ProductAttributes productAttributes in product.Metadata)
                        {
                            await sqlConnection.ExecuteAsync(insertProductMetadataQuery, new
                            {
                                productId,
                                productAttributes.Key,
                                productAttributes.Value
                            }, sqlTransaction);
                        }

                    }
                    return productId;
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BaseProduct>> GetAllAsync(BaseProduct product)
        {
            try
            {
                //EVAL : Prefer to use store procedure with TVP as parameter to address the below
                // search criteria
                string searchProductQuery = @"SELECT 
                Product.InstanceId AS InstanceId, Product.Name AS Name,
                Product.Description AS Description, Product.ValidSkus as ValidSkus,
                Product.ProductImageUris as ProductImageUris,
                Metadata.Key as MetadataKey, Metadata.Value as MetadataValue
                FROM [Instances].[Products] Product
                JOIN [Instances].[ProductAttributes] Metadata 
                ON Metadata.InstanceId = Product.InstanceId 
                WHERE 1=1";

                var dynamicParameters = new DynamicParameters();

                if(product.Name != null)
                {
                    searchProductQuery += " AND Product.Name = @productName ";
                    dynamicParameters.Add("productName", product.Name);
                }
                if (product.ValidSkus != null)
                {
                    searchProductQuery += " AND CONTAINS(Product.ValidSkus = @productValidSkus) ";
                    dynamicParameters.Add("productValidSkus", product.ValidSkus);
                }

                if (product.Metadata != null && product.Metadata.Count > 0)
                {
                    foreach(ProductAttributes productAttributes in product.Metadata)
                    {
                        searchProductQuery += " AND (Metadata.Key = @metadataKey AND Metadata.Value = @metadataValue ) ";
                        dynamicParameters.Add("metadataKey", productAttributes.Key);
                        dynamicParameters.Add("metadataValue", productAttributes.Value);
                    }
                }


                    return await _SqlExecutor.ExecuteAsync(async (sqlConnection, sqlTransaction) =>
                {
                    return await sqlConnection.QueryAsync(searchProductQuery,
                         (BaseProduct baseProduct, ProductAttributes metadata) =>
                         {
                             baseProduct.Metadata.Add(metadata);
                             return baseProduct;
                         },dynamicParameters,sqlTransaction);

                }) as List<BaseProduct>;

            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Not implemented
        public Task<int> DeleteAsync(Models.Products.BaseProduct entity)
        {
            throw new NotImplementedException();
        }

        public Task<Models.Products.BaseProduct> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<int> UpdateAsync(Models.Products.BaseProduct entity)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
