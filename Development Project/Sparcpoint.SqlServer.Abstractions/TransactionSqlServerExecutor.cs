using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sparcpoint.SqlServer.Abstractions
{
    public class TransactionSqlServerExecutor : ISqlExecutor
    {
        private readonly SqlTransaction _Transaction;
        public TransactionSqlServerExecutor(SqlTransaction transaction)
        {
            _Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        public T Execute<T>(Func<IDbConnection, IDbTransaction, T> command)
            => command(_Transaction.Connection, _Transaction);

        public async Task ExecuteAsync(Func<IDbConnection, IDbTransaction, Task> command)
            => await command(_Transaction.Connection, _Transaction);

        public async Task<T> ExecuteAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> command)
            => await command(_Transaction.Connection, _Transaction);
    }
}
