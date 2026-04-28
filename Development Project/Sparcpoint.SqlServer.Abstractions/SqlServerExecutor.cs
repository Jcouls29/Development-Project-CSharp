using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Sparcpoint.SqlServer.Abstractions
{
    public class SqlServerExecutor : ISqlExecutor
    {
        private readonly string _ConnectionString;

        public SqlServerExecutor(string connectionString)
        {
            _ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public T Execute<T>(Func<IDbConnection, IDbTransaction, T> command)
        {
            using (var sqlConn = Open())
            using (var sqlTrans = sqlConn.BeginTransaction())
            {
                try
                {
                    var result = command(sqlConn, sqlTrans);
                    sqlTrans.Commit();
                    return result;
                }
                catch
                {
                    // EVAL: Rollback is critical — without this, an exception leaves the transaction
                    // open until the connection is disposed, which can cause deadlocks under load.
                    sqlTrans.Rollback();
                    throw;
                }
            }
        }

        public async Task ExecuteAsync(Func<IDbConnection, IDbTransaction, Task> command)
        {
            using (var sqlConn = await OpenAsync())
            using (var sqlTrans = sqlConn.BeginTransaction())
            {
                try
                {
                    await command(sqlConn, sqlTrans);
                    sqlTrans.Commit();
                }
                catch
                {
                    sqlTrans.Rollback();
                    throw;
                }
            }
        }

        public async Task<T> ExecuteAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> command)
        {
            using (var sqlConn = await OpenAsync())
            using (var sqlTrans = sqlConn.BeginTransaction())
            {
                try
                {
                    var result = await command(sqlConn, sqlTrans);
                    sqlTrans.Commit();
                    return result;
                }
                catch
                {
                    sqlTrans.Rollback();
                    throw;
                }
            }
        }

        private SqlConnection Open()
        {
            var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            return connection;
        }

        private async Task<SqlConnection> OpenAsync()
        {
            var connection = new SqlConnection(_ConnectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
