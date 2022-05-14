using Dapper;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Threading.Tasks;


//EVAL: This class was never used and was as far as I got tying to use SqlServerExecuter :-(

namespace Interview.Web.Repository
{
    public class SqlServerRepository<T>
        where T : new()
    {
        private readonly ISqlExecutor _Executor;

        public SqlServerRepository(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public Task FindAsync(int id)
        {
            const string SQL = @"server=PEAK; database=sparcpoint.Inventory.Database;  Integrated Security=true;";

            return _Executor.ExecuteAsync(
              (sqlConn, sqlTrans) => sqlConn.QuerySingle(SQL, new { Id = id }, transaction: sqlTrans));
        }
    }
}