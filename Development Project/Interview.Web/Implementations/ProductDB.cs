using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Threading.Tasks;
using Dapper;
using Interview.Web.Models;

namespace Interview.Web
{
    public class ProductDB : IProductDB
    {
        private readonly ISqlExecutor _Executor;

        public ProductDB(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<int> AddProductAsync(Product product)
        {
            if (product == null)   
            {
                throw new ArgumentNullException(nameof(product));
            }
            //EVAL: will need more validation on the product properties
            const string SQL = @"
                INSERT INTO [dbo].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus])
                Values (@Name, @Description, @ProductImageUris, @ValidSkus)
                ;
                DECLARE @ProductId INT - SCOPE_IDENTITY();
                ;
                INSERT INTO [dob].[ProductAttributes] ([ProductID], [Key], [Value])
                SELECT @ProductID, [Key], [Value]
                FROM @ProductAttributes
                ;
                INSERT INTO [dob].[ProductCategories] ([ProductID], [CategoryID])
                SELECT @ProductId, [CategoryID]
                FROM @ProductCategories
                ;
                SELECT @ProductId, 
                ;
            ";

            return await _Executor
                .ExecuteAsync(async (sqlConn, sqlTrans) =>
                {
                    return await sqlConn.QuerySingleAsync<int>(SQL, new
                    {
                        product.Name,
                        Description = product.Description ?? "",
                        ProductImageUris = product.ProductImageUris ?? "",
                        ValidSkus = product.ValidSkus ?? "",
                        product.Attributes,
                        product.Categories
                    }, sqlTrans);
                });
        }
    }
}