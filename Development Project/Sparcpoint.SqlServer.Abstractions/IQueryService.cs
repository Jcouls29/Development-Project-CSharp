using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.SqlServer.Abstractions
{
    public interface IQueryService
    {
        IEnumerable<T> Query<T>(string sql, object param = null, CommandType? commandType = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null,
            CommandType? commandType = null);
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType? commandType = null);
    }
}