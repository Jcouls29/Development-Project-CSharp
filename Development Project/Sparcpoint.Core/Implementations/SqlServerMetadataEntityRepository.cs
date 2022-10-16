using Dapper;
using Sparcpoint.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations
{
    public abstract class SqlServerMetadataEntityRepository
    {
        internal readonly ISqlExecutor _SqlExecutor;
        internal readonly string _MetadataTableName;

        public SqlServerMetadataEntityRepository(ISqlExecutor sqlExecutor, string metadataTableName)
        {
            string metadataTableNameClean = metadataTableName?.Replace("[", "")?.Replace("]", "");

            PreConditions.ParameterNotNull(sqlExecutor, nameof(sqlExecutor));
            PreConditions.StringNotNullOrWhitespace(metadataTableName
                                                   , nameof(metadataTableName));

            _SqlExecutor = sqlExecutor;
            _MetadataTableName = metadataTableNameClean;
        }

        internal async Task AddMetadataAsync(MetadataEntity metadataEntity)
        {
            string sql = $@"INSERT INTO [Instances].[{_MetadataTableName}] 
                ([InstanceId], [Key], [Value])
                VALUES
                (@InstanceId, @Key, @Value);";

            List<Task> insertTasks = new List<Task>();

            foreach (KeyValuePair<string, string> metadata in metadataEntity.Metadata)
                insertTasks.Add(_SqlExecutor.ExecuteAsync(async (sqlConnection, sqlTransaction) =>
                {
                    await sqlConnection.ExecuteAsync(sql, new
                    {
                        metadataEntity.InstanceId,
                        metadata.Key,
                        metadata.Value,
                    }, sqlTransaction);
                }));

            await Task.WhenAll(insertTasks);
        }
    }
}
