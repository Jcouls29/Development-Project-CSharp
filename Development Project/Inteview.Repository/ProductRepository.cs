using Interview.Entities;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Inteview.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }
        public int AddProduct(Product product)
        {
            string sql = @"
                INSERT INTO [Instances].[Products]
                            ([Name]
                            ,[Description]
                            ,[ProductImageUris]
                            ,[ValidSkus]
                            ,[CreatedTimestamp])
                        VALUES
                            (@Name
                            ,@Description
                            ,@ProductImageUris
                            ,@ValidSkus,
                            ,@CreatedTimestamp";

            var parms = new List<SqlParameter>();
            parms.Add(new SqlParameter("@Name", product.Name));
            parms.Add(new SqlParameter("@Description", product.Description));
            parms.Add(new SqlParameter("@ProductImageUris", product.ProductImageUris));
            parms.Add(new SqlParameter("@ValidSkus", product.ValidSkus));
            parms.Add(new SqlParameter("@CreatedTimestamp", product.CreatedTimestamp));

            //got this far in the interview assessment
            //_sqlExecutor.Execute

            return 1;
        }
    }
}
