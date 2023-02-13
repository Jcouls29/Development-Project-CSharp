﻿using System;
using System.Data;
using System.Data.SqlClient;
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

        public SqlServerExecutor(SqlServerOptions sqlServerOptions)
        {
            if (sqlServerOptions == null) throw new ArgumentNullException(nameof(sqlServerOptions));
            if (string.IsNullOrEmpty(sqlServerOptions.ConnectionString))
                throw new ArgumentException("ConnectionString property must be provided", nameof(sqlServerOptions));

            _ConnectionString = sqlServerOptions.ConnectionString;
        }

        public T Execute<T>(Func<IDbConnection, IDbTransaction, T> command)
        {
            using (var sqlConn = Open())
            using (var sqlTrans = sqlConn.BeginTransaction())
            {
                var result = command(sqlConn, sqlTrans);
                sqlTrans.Commit();
                return result;
            }
        }

        public async Task ExecuteAsync(Func<IDbConnection, IDbTransaction, Task> command)
        {
            using (var sqlConn = await OpenAsync())
            using (var sqlTrans = sqlConn.BeginTransaction())
            {
                await command(sqlConn, sqlTrans);
                sqlTrans.Commit();
            }
        }

        public async Task<T> ExecuteAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> command)
        {
            using (var sqlConn = await OpenAsync())
            using (var sqlTrans = sqlConn.BeginTransaction())
            {
                var result = await command(sqlConn, sqlTrans);
                sqlTrans.Commit();
                return result;
            }
        }

        private SqlConnection Open()
        {
            SqlConnection connection = new SqlConnection(_ConnectionString);
            connection.Open();
            return connection;
        }

        private async Task<SqlConnection> OpenAsync()
        {
            SqlConnection connection = new SqlConnection(_ConnectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
