using Microsoft.Extensions.DependencyInjection;
using Sparcpoint.SqlServer.Abstractions;
using System.Data.SqlClient;

namespace Sparcpoint.Inventory.Application.Data
{
    public class SqlExecutorFactory : ISqlExecutorFactory
    {
        private readonly string _ConnectionString;
        public SqlExecutorFactory(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        public ISqlExecutor Create()
        {
            return Create(_ConnectionString);
        }
        public ISqlExecutor Create(string connectionString)
        {
            return new SqlServerExecutor(connectionString);
        }
        public ISqlExecutor Create(SqlTransaction sqlTransaction)
        {
            return new TransactionSqlServerExecutor(sqlTransaction);
        }
    }

    public static class SqlExecutorFactoryExtensions
    {
        public static IServiceCollection AddSqlExecutorFactory(this IServiceCollection services, string connectionString)
        {
            return services.AddSingleton<ISqlExecutorFactory>(serviceProvider => new SqlExecutorFactory(connectionString));
        }
    }
}
