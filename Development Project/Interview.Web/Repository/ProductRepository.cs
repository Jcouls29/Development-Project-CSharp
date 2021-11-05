using Dapper;
using Interview.Web.Model;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Web.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _sqlExecutor;
        public ProductRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        #region Product

        /// <summary>
        /// Search product based on criteria
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Task<List<Product>> SearchProduct(Product product)
        {
            var queryProvider = new SqlServerQueryProvider();
            var appendAnd = false;
            var appendWhere = true;

            var query = new StringBuilder($"select p.InstanceId,p.Name,p.Description,P.ProductImageUris,p.ValidSkus,c.Name as CategoryName from Instances.Products p inner join Instances.ProductCategories pc on  pc.InstanceId=p.InstanceId inner join Instances.Categories c on c.InstanceId=pc.CategoryInstanceId  ");

            //Dynamically forming query based on filter criteria
            if (!string.IsNullOrEmpty(product.CategoryName))
            {
                if (appendWhere)
                {
                    query.Append($" where ");
                    appendWhere = false;
                }

                query.Append($"C.Name = @CategoryName");
                appendAnd = true;
            }

            if (!string.IsNullOrEmpty(product.Description))
            {
                if (appendWhere)
                {
                    query.Append($" where ");
                    appendWhere = false;
                }


                if (appendAnd)
                    query.Append($" and ");

                query.Append($"P.Description = @Description");
            }

            return _sqlExecutor.ExecuteAsync(async (conn, trn) =>
            {
                var result = (await conn.QueryAsync<Product>(query.ToString(), product, trn));
                return result.ToList();
            });
        }

        /// <summary>
        /// //Add produc and Categories
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Task Add(Product item)
        {
            var productQuery = $"insert into Instances.Products (Name,Description,ProductImageUris,ValidSkus) values (@Name,@Description,@ProductImageUris,@ValidSkus)";

            var categoryQuery = $"insert into Instances.Categories (Name,Description) values (@CategoryName,@Description)";

            return _sqlExecutor.ExecuteAsync(async (conn, trn) =>
            {

                await conn.ExecuteAsync(productQuery, item, trn);

                //get the idenity of the product
                var instanceId = Convert.ToInt64(conn.ExecuteScalar<object>("SELECT @@IDENTITY", null, trn));

                await conn.ExecuteAsync(categoryQuery, item, trn);

                //get the idenity of the category
                var categoryInstanceId = Convert.ToInt64(conn.ExecuteScalar<object>("SELECT @@IDENTITY", null, trn));

                var productCategories = new { InstanceId = instanceId, CategoryInstanceId = categoryInstanceId };

                await conn.ExecuteAsync("insert into Instances.ProductCategories values(@instanceId,@categoryInstanceId)", productCategories, trn);

            });
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCount()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Product> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Remove(Product item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
