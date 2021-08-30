using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sparcpoint.SqlServer.Abstractions
{
    public interface ISqlExecutor
    {
        Task ExecuteAsync(Func<IDbConnection, IDbTransaction, Task> command);
        Task<T> ExecuteAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> command);
        T Execute<T>(Func<IDbConnection, IDbTransaction, T> command);
    }
}
