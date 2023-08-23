using Sparcpoint.SqlServer.Abstractions;
using System.Data.SqlClient;

namespace Sparcpoint.Inventory.Application.Data
{
    public interface ISqlExecutorFactory
    {
        ISqlExecutor Create();
        ISqlExecutor Create(string connectionString);
        ISqlExecutor Create(SqlTransaction sqlTransaction);
    }
}
