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
    public class SqlServerCategoryRepository : ICategoryRepository
    {
        //EVAL: This doesn't feel like a particularly elegant solution. With more time, I may consider some sort
        //      of ICategoryValidator service.
        private const int NAME_MAX_LENGTH = 64;
        private const int DESCRIPTION_MAX_LENGTH = 256;

        private readonly ISqlExecutor _SqlExecutor;

        public SqlServerCategoryRepository(ISqlExecutor sqlExecutor)
        {
            PreConditions.ParameterNotNull(sqlExecutor, nameof(sqlExecutor));

            _SqlExecutor = sqlExecutor;
        }

        public async Task<int> AddCategoryAsync(Category category)
        {
            ValidateAddCategoryParameter(category, nameof(category));

            int categoryId = await AddOnlyCategoryAsync(category);

            category.InstanceId = categoryId;

            //Null coalesce into 0 to check for both null and empty collection in one condition
            if ((category.Metadata?.Count ?? 0) > 0)
                await AddCategoryMetadataAsync(category);

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

        private async Task AddCategoryMetadataAsync(Category category)
        {
            string sql = @"INSERT INTO [Instances].[CategoryAttributes] 
                ([InstanceId], [Key], [Value])
                VALUES
                (@InstanceId, @Key, @Value);";

            List<Task> insertTasks = new List<Task>();

            foreach (KeyValuePair<string, string> metadata in category.Metadata)
                insertTasks.Add(_SqlExecutor.ExecuteAsync(async (sqlConnection, sqlTransaction) =>
                {
                    await sqlConnection.ExecuteAsync(sql, new
                    {
                        category.InstanceId,
                        metadata.Key,
                        metadata.Value,
                    }, sqlTransaction);
                }));

            await Task.WhenAll(insertTasks);
        }
    }
}
