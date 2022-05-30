using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private const string procAddProduct = "dbo.usp_AddProduct";

        //EVAL: usp_SearchProduct procedure not created yet
        private const string procSearchProduct = "dbo.usp_SearchProduct";

        private readonly ILogger<ProductRepository> _logger;
        private readonly IOptions<ConnConfig> _connConfig;

        public ProductRepository(ILogger<ProductRepository> logger, IOptions<ConnConfig> connConfig)
        {
            _logger = logger;
            _connConfig = connConfig;
        }

        public async Task<int> AddProduct(Product product)
        {
            try
            {
                string connStr = _connConfig.Value.DataConnection;
                using (IDbConnection db = new SqlConnection(connStr))
                {
                    var queryParams = new DynamicParameters();
                    queryParams.Add("@AddedProductId", DbType.Int32, direction: ParameterDirection.Output);
                    queryParams.Add("@Name", product.Name);
                    queryParams.Add("@Description", product.Description);
                    queryParams.Add("@ProductImageUris", product.ProductImageUris);
                    queryParams.Add("@ValidSkus", product.ValidSkus);
                    queryParams.Add("@CreatedTimestamp", DateTime.Now);

                    var result = await db.ExecuteScalarAsync<int>(procAddProduct, queryParams, commandType: CommandType.StoredProcedure);

                    return queryParams.Get<int>("WaterLogID");
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                throw;
            }
        }

        //EVAL: SearchProducts not functional as usp_SearchProduct procedure has not been created
        public async Task<List<Product>> SearchProducts()
        {
            try
            {
                string connStr = _connConfig.Value.DataConnection;
                using (IDbConnection db = new SqlConnection(connStr))
                {
                    var productList = await db.QueryAsync<Product>(procSearchProduct, null, commandType: CommandType.StoredProcedure);

                    return productList.AsList();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                throw;
            }
        }

    }
}
