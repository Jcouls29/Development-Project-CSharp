using Dapper;
using Inventory.Data;
using Microsoft.EntityFrameworkCore;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Inventory.BusinessServices.Services
{
    public class ProductService
    {
        private readonly IProductService _productService;
        private readonly InventoryDataContext _inventoryDataContext;
        public ProductService(InventoryDataContext inventoryDataContext, IProductService productService )
        {
            _productService = productService;
            _inventoryDataContext = inventoryDataContext;
        }

        /// <summary>
        /// Add Product 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> AddProductAsync(Product product)
        {
            try
            {
                if (product != null)
                {
                    // using context
                    _inventoryDataContext.Products.Add(new Data.Product {
                        Name = product.Name,
                        Description = product.Description,
                        ValidSkus = product.ValidSkus,
                        ProductImageUris = product.ProductImageUris,
                        Type = product.Type
                    });
                    await _inventoryDataContext.SaveChangesAsync();


                    // using dappler. Tried using existing SQL Abstractions SQL Server extraction. Running out of 2hrs 
                    string procedure = "sp_AddProduct";
                    await SaveInDB(procedure, product);
                    return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK };
                }
                else
                {
                    return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.NotFound };
                }
            }
            catch (Exception)
            {
                //TODO : logger, handle errors
                throw;
            }
        }

        
        public async Task<HttpResponseMessage> AddTransactionAsync(InventoryTransaction inventoryTransaction)
        {
            try
            {
                // using context
                _inventoryDataContext.InventoryTransaction.Add(new Data.InventoryTransaction
                {
                    CategoryId = inventoryTransaction.CategoryId,
                    ProductInstanceId = inventoryTransaction.ProductInstanceId,
                    Quantity = inventoryTransaction.Quantity,
                });
                await _inventoryDataContext.SaveChangesAsync();

                // Add Range for multiple

                // dappler
                string procedure = "sp_AddInventoryTransaction";
                await SaveInDBTransaction(procedure, inventoryTransaction);
                return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK };
            }
            catch (Exception)
            {
                //TODO : logger, handle errors
                throw;
            }
        }

        /// <summary>
        /// This method is for deleting inventory transactions one way not effective.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteInventoryTransactionAsync(int id)
        {
            // dappler
            var sql = "DELETE FROM InventoryTransactions WHERE TransactionId = @Id";
            using (var connection = new SqlConnection("sql connection string"))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { TransactionId = id });
                return result;
            }
        }
        /// <summary>
        /// Common function
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task SaveInDB(string procedure, object param)
        {
             using var connection = new SqlConnection("sql connection string");
             await connection.OpenAsync();
            // params will be @values,not effective.
            await connection.ExecuteAsync(procedure, param, commandType: CommandType.StoredProcedure);
            await connection.CloseAsync();
        }

        public async Task SaveInDBTransaction(string procedure, object param)
        {
            using var connection = new SqlConnection("sql connection string");
            await using var transaction = await connection.BeginTransactionAsync();
            await connection.OpenAsync();
            // params will be @values,not effective.
            await connection.ExecuteAsync(procedure, param, commandType: CommandType.StoredProcedure);
            await connection.CloseAsync();
        }

        //public List<Product> GetProductAssosicationsAsync(int instanceId)
        //{

        //    try
        //    {
        //        var products = _inventoryDataContext.Products.Where(p => p.Id == instanceId)
        //                                       .Include(p => p.Attribute)
        //                                       .Include(p => p.CategoryAttribute).ToList();
        //        return products; notime

        //    }
        //    catch(Exception) { }
        //    //handle later


        //}
        public void SearchProducts()
        {
            SqlServerQueryProvider queryProvider = new SqlServerQueryProvider();
            //queryProvider.Join("")
        }


    }
}