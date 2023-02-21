using Dapper;
using Sparcpoint.Inventory.Core.Requests;
using Sparcpoint.Inventory.Core.Response;
using Sparcpoint.Inventory.Repository.Interfaces;
using Sparcpoint.InventoryService.Common.Extensions;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Sparcpoint.Inventory.Repository.Services
{
    public class ProductService : IProductService
    {
        private readonly SqlServerExecutor _sqlServerExecutor;
        public ProductService(SqlServerExecutor sqlServerExecutor) 
        {
            _sqlServerExecutor = sqlServerExecutor.ThrowIfNull(nameof(sqlServerExecutor));
        }

        public async Task<int> AddProductAsync(AddProductRequest addProductRequest)
        {
            //EVAL: This needs to be put in a constants file
            var query = @"INSERT INTO [Inventory].[Instances].[Products] (Name, Description, ProductImageUris, ValidSkus) " +
                "OUTPUT inserted.InstanceId " +
                "VALUES (@Name, @Description, @ProductImageUris, @ValidSkus)";

            return await _sqlServerExecutor.ExecuteAsync(async (conn, tran) =>
            {
                var instanceId = await conn.QuerySingleAsync<int>(query, addProductRequest, tran);
                return instanceId;
            });
        }

        public async Task<List<int>> AddProductAttributesAsync(List<AddProductAttributesRequest> addProductAttributesRequest)
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"INSERT INTO [Inventory].[Instances].[ProductAttributes] (InstanceId, [Key], [Value]) OUTPUT inserted.InstanceId VALUES ");

            foreach (var productAttribute in addProductAttributesRequest)
            {
                query.Append($"({productAttribute.InstanceId}, '{productAttribute.Key}', '{productAttribute.Value}'),");
            }

            var result = await _sqlServerExecutor.ExecuteAsync(async (conn, tran) =>
            {
                var result = await conn.QueryAsync<int>(query.ToString().Trim().TrimEnd(",".ToCharArray()), transaction: tran);
                return result.ToList();
            });

            return await Task.FromResult(result);
        }
    }
}
