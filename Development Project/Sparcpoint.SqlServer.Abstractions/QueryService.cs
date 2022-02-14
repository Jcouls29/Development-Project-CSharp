using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.SqlServer.Abstractions
{
    public class QueryService : IQueryService
    {
        private readonly SqlServerOptions _sqlServerOptions;

        public QueryService(SqlServerOptions sqlServerOptions)
        {
            _sqlServerOptions = sqlServerOptions;
        }

        public IEnumerable<T> Query<T>(string sql, object param = null, CommandType? commandType = null)
        {
            using (var sqlConn = Open())
            {
                return sqlConn.Query<T>(sql, param, commandType: commandType);
            }
 
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType? commandType = null)
        {
            using (var sqlConn = await OpenAsync())
            {
               
                return await sqlConn.QueryAsync<T>(sql, param, commandType: commandType);
            }
        }
        public async Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType? commandType = null)
        {
            using (var sqlConn = await OpenAsync())
            {

                 return await sqlConn.ExecuteScalarAsync<T>(sql, param, commandType: commandType);
            }
        }

        private SqlConnection Open()
        {
            SqlConnection connection = new SqlConnection(_sqlServerOptions.ConnectionString);
            connection.Open();
            return connection;
        }

        private async Task<SqlConnection> OpenAsync()
        {
            SqlConnection connection = new SqlConnection(_sqlServerOptions.ConnectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
