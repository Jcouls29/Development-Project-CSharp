using Dapper;
using Sparcpoint.Abstract;
using Sparcpoint.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations
{
    public class SqlServerCategoryRepository : SqlServerMetadataEntityRepository, ICategoryRepository
    {
        private const string METADATA_TABLE_NAME = "CategoryAttributes";

        //EVAL: This doesn't feel like a particularly elegant solution. With more time, I may consider some sort
        //      of ICategoryValidator service.
        private const int NAME_MAX_LENGTH = 64;
        private const int DESCRIPTION_MAX_LENGTH = 256;


        public SqlServerCategoryRepository(ISqlExecutor sqlExecutor) : base(sqlExecutor, METADATA_TABLE_NAME)
        {
        }

        public async Task<int> AddCategoryAsync(Category category)
        {
            ValidateAddCategoryParameter(category, nameof(category));

            int categoryId = await AddOnlyCategoryAsync(category);

            category.InstanceId = categoryId;

            //Null coalesce into 0 to check for both null and empty collection in one condition
            if ((category.Metadata?.Count ?? 0) > 0)
                await AddMetadataAsync(category);

            return categoryId;
        }

        private void ValidateAddCategoryParameter(Category category, string parameterName)
        {
            PreConditions.ParameterNotNull(category, parameterName);

            PreConditions.StringNotNullOrWhitespace(category.Name, nameof(category.Name));
            PreConditions.StringNotNullOrWhitespace(category.Description, nameof(category.Description));

            PreConditions.StringLengthDoesNotExceed(category.Name, NAME_MAX_LENGTH, nameof(category.Name));
            PreConditions.StringLengthDoesNotExceed(category.Description, DESCRIPTION_MAX_LENGTH, nameof(category.Description));
        }

        private async Task<int> AddOnlyCategoryAsync(Category category)
        {
            string sql = @"INSERT INTO [Instances].[Categories] 
                ([Name], [Description])
                VALUES
                (@Name, @Description);
                SELECT SCOPE_IDENTITY();";

            return await _SqlExecutor.ExecuteAsync<int>(async (sqlConnection, sqlTransaction) =>
            {
                return await sqlConnection.QuerySingleAsync<int>(sql, new
                {
                    category.Name,
                    category.Description,
                }, sqlTransaction);
            });
        }
    }
}
