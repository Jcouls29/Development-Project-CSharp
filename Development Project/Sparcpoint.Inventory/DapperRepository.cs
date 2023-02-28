using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using SqlMapper = Dapper.SqlMapper;

namespace Sparcpoint.Inventory
{
    /// <summary>
    /// I'm more comfortable with EF, but the requirements specified that ISqlExecutor should be used as well as Dapper, so I roughed this incomplete repo in.
    /// </summary>
    public class DapperRepository
    {
        private readonly ISqlExecutor executor;

        public DapperRepository(ISqlExecutor executor)
        {
            this.executor = executor;
        }

        public async Task<int> AddProductAsync(DapperProduct product)
        {
            const string sql =
                @"INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus]) VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                                DECLARE @InstanceId INT = SCOPE_IDENTITY();
                                INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value]) SELECT @InstanceId, [Key], [Value] FROM @ProductAttributes;  
                                INSERT INTO [Instances].[Categories] ([InstanceId], [Name], [Description]) SELECT @InstanceId, [Name], [Description] FROM @Categories;
                                SELECT @InstanceId";

            // This returns a NotSupportedException:
            // return await this.executor.ExecuteAsync(async (conn, trans) => await conn.InsertAsync(product, trans));

            return await this.executor.ExecuteAsync(async (conn, trans)
                => await SqlMapper.QuerySingleAsync<int>(conn, sql, new
                {
                    product.Name,
                    product.Description,
                    product.ProductImageUris,
                    product.ValidSkus,
                    ProductAttributes = this.CreateTableValuedParameter(product.ProductAttributes, "[dbo].[CustomAttributeList]", "Key", "Value"),
                    ProductCategories = this.CreateTableValuedParameter(product.ProductCategories, "[Instances].[CorrelatedListItemList]", "Name", "Description")
                }, trans));
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await this.executor.ExecuteAsync(async (conn, trans) =>
                await SqlMapper.QueryAsync<Product>(conn, "SELECT * FROM [Instances].[Products]"));
        }

        private SqlMapper.ICustomQueryParameter CreateTableValuedParameter(IDictionary<string, string> dict, string tableType, string colName1, string colName2)
        {
            var dt = new DataTable(tableType);

            dt.Columns.Add(colName1, typeof(string));
            dt.Columns.Add(colName2, typeof(string));

            foreach (var kvp in dict)
                dt.Rows.Add(kvp.Key, kvp.Value);

            return dt.AsTableValuedParameter(tableType);
        }
    }
}
