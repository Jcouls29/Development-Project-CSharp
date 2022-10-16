﻿using Sparcpoint.Abstract;
using Sparcpoint.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Sparcpoint.Implementations
{
    public class SqlServerProductRepository : SqlServerMetadataEntityRepository, IProductRepository
    {
        private const string METADATA_TABLE_NAME = "ProductAttributes";

        //EVAL: This doesn't feel like a particularly elegant solution. With more time, I may consider some sort
        //      of IProductValidator service.
        private const int NAME_MAX_LENGTH = 256;
        private const int DESCRIPTION_MAX_LENGTH = 256;

        public SqlServerProductRepository(ISqlExecutor sqlExecutor) : base(sqlExecutor, METADATA_TABLE_NAME)
        {
        }

        public async Task<int> AddProductAsync(Product product)
        {
            ValidateAddProductParameter(product, nameof(product));

            int productId = await AddOnlyProductAsync(product);

            product.InstanceId = productId;

            //Null coalesce into 0 to check for both null and empty collection in one condition
            if ((product.Metadata?.Count ?? 0) > 0)
                await AddMetadataAsync(product);

            return productId;
        }

        private void ValidateAddProductParameter(Product product, string parameterName)
        {
            PreConditions.ParameterNotNull(product, parameterName);

            PreConditions.StringNotNullOrWhitespace(product.Name, nameof(product.Name));
            PreConditions.StringNotNullOrWhitespace(product.Description, nameof(product.Description));
            PreConditions.StringNotNullOrWhitespace(product.ProductImageUris, nameof(product.ProductImageUris));
            PreConditions.StringNotNullOrWhitespace(product.ValidSkus, nameof(product.ValidSkus));

            PreConditions.StringLengthDoesNotExceed(product.Name, NAME_MAX_LENGTH, nameof(product.Name));
            PreConditions.StringLengthDoesNotExceed(product.Description, DESCRIPTION_MAX_LENGTH, nameof(product.Description));
        }

        private async Task<int> AddOnlyProductAsync(Product product)
        {
            string sql = @"INSERT INTO [Instances].[Products] 
                ([Name], [Description], [ProductImageUris], [ValidSkus])
                VALUES
                (@Name, @Description, @ProductImageUris, @ValidSkus);
                SELECT SCOPE_IDENTITY();";

            return await _SqlExecutor.ExecuteAsync<int>(async (sqlConnection, sqlTransaction) =>
            {
                return await sqlConnection.QuerySingleAsync<int>(sql, new
                {
                    product.Name,
                    product.Description,
                    product.ProductImageUris,
                    product.ValidSkus,
                }, sqlTransaction);
            });
        }

        
    }
}
