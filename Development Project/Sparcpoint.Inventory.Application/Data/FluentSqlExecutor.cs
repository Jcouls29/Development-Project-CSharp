using Dapper;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Application.Data
{
    public class SqlBuilderContext
    {
        public List<string> SelectedColumns { get; set; } = new List<string>();
        public string FromTable { get; set; }
        public List<string> Joins { get; set; } = new List<string>();
        public string WhereClause { get; set; }

        public SqlBuilderContext()
        {

        }
    }

    public class FluentSqlExecutor
    {
        private readonly ISqlExecutorFactory _factory;
        private readonly ISqlExecutor _executor;
        private string _query;
        private readonly Dictionary<string, object> _parameters;

        public FluentSqlExecutor(ISqlExecutorFactory factory)
        {
            _factory = factory;
            _executor = factory.Create();
            _parameters = new Dictionary<string, object>();
        }

        public FluentSqlExecutor Select(params string[] columns)
        {
            _query = $"SELECT {string.Join(", ", columns)}";
            return this;
        }

        public FluentSqlExecutor From(string tableName)
        {
            _query += $" FROM {tableName}";
            return this;
        }

        public FluentSqlExecutor Join(string table, string onClause)
        {
            _query += $" JOIN {table} ON {onClause}";
            return this;
        }

        public FluentSqlExecutor Where(string condition)
        {
            _query += $" WHERE {condition}";
            return this;
        }

        public FluentSqlExecutor WithParameters(Dictionary<string, object> parameters)
        {
            foreach (var param in parameters)
            {
                _parameters[param.Key] = param.Value;
            }
            return this;
        }

        public FluentSqlExecutor InsertInto(string tableName, Dictionary<string, object> columnsValues)
        {
            var columns = string.Join(", ", columnsValues.Keys);
            var values = string.Join(", ", columnsValues.Values.Select(v => $"@{v}"));
            _query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
            WithParameters(columnsValues);
            return this;
        }

        public FluentSqlExecutor DeleteFrom(string tableName)
        {
            _query = $"DELETE FROM {tableName}";
            return this;
        }

        public async Task<int> ExecuteNonQueryAsync()
        {
            return await _executor.ExecuteAsync(async (conn, trans) =>
            {
                return await conn.ExecuteAsync(_query, _parameters, transaction: trans);
            });
        }

        public async Task<T> ToSingleAsync<T>()
        {
            return await _executor.ExecuteAsync(async (conn, trans) =>
            {
                return await conn.QuerySingleOrDefaultAsync<T>(_query, _parameters, transaction: trans);
            });
        }

        public async Task<IEnumerable<T>> ToListAsync<T>()
        {
            return await _executor.ExecuteAsync(async (conn, trans) =>
            {
                return await conn.QueryAsync<T>(_query, _parameters, transaction: trans);
            });
        }

    }
}
