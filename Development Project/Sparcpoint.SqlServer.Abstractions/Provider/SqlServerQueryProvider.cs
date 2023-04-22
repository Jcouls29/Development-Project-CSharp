using System;
using System.Collections.Generic;

namespace Sparcpoint.SqlServer.Abstractions
{
    public class SqlServerQueryProvider
    {
        private string _TargetTableAlias;
        private int _CurrentParameterId = 0;

        private readonly JoinTables _Joins;
        private readonly WhereClause _Where;
        private readonly OrderByClause _OrderBy;
        private Dictionary<string, object> _Parameters;

        public SqlServerQueryProvider()
        {
            _TargetTableAlias = null;

            _Joins = new JoinTables();
            _Where = new WhereClause();
            _OrderBy = new OrderByClause();

            _Parameters = new Dictionary<string, object>();
        }

        public string JoinClause => _Joins.ToString();
        public string WhereClause => _Where.ToString();
        public string OrderByClause => _OrderBy.ToString();
        public Dictionary<string, object> Parameters => new(_Parameters);

        public SqlServerQueryProvider SetTargetTableAlias(string tableAlias)
            => Execute(() => _TargetTableAlias = SqlServerValidation.SanitizeTableName(tableAlias));

        public SqlServerQueryProvider ClearTargetTableAlias()
            => Execute(() => _TargetTableAlias = null);

        public SqlServerQueryProvider OrderBy(string columnName, OrderByClause.OrderByDirection direction)
            => Execute(() => _OrderBy.Add(FormatColumnName(columnName), direction));

        public SqlServerQueryProvider OrderByDescending(string columnName)
            => OrderBy(columnName, Abstractions.OrderByClause.OrderByDirection.Descending);

        public SqlServerQueryProvider OrderByAscending(string columnName)
            => OrderBy(columnName, Abstractions.OrderByClause.OrderByDirection.Ascending);

        public SqlServerQueryProvider Join(string tableName, string abbv, string onClause)
            => Execute(() => _Joins.Add(tableName, abbv, onClause));

        public SqlServerQueryProvider Join(string clause)
            => Execute(() => _Joins.Add(clause));

        public SqlServerQueryProvider Where(string clause)
            => Execute(() => _Where.Add(clause));

        public SqlServerQueryProvider WhereIn(string columnName, string expression)
            => WhereInnerExpression(columnName, expression, "IN");

        public SqlServerQueryProvider WhereNotIn(string columnName, string expression)
            => WhereInnerExpression(columnName, expression, "NOT IN");

        public SqlServerQueryProvider WhereExists(string columnName, string expression)
            => WhereInnerExpression(columnName, expression, "EXISTS");

        public SqlServerQueryProvider WhereNotExists(string columnName, string expression)
            => WhereInnerExpression(columnName, expression, "NOT EXISTS");

        private SqlServerQueryProvider WhereInnerExpression(string columnName, string expression, string op)
            => Execute(() =>
            {
                columnName = FormatColumnName(SqlServerValidation.SanitizeColumnName(columnName));
                _Where.Add($"{columnName} {op} {expression}");
            });

        public SqlServerQueryProvider WhereIsNull(string columnName)
            => Execute(() =>
            {
                columnName = FormatColumnName(SqlServerValidation.SanitizeColumnName(columnName));
                _Where.Add($"{columnName} IS NULL");
            });

        public SqlServerQueryProvider WhereIsNotNull(string columnName)
            => Execute(() =>
            {
                columnName = FormatColumnName(SqlServerValidation.SanitizeColumnName(columnName));
                _Where.Add($"{columnName} IS NOT NULL");
            });

        public SqlServerQueryProvider WhereIsNull(string parameterName, object value)
            => Execute(() =>
            {
                parameterName = SqlServerValidation.SanitizeParameterName(parameterName);
                _Where.Add($"{parameterName} IS NULL");
                AddParameter(parameterName, value);
            });

        public SqlServerQueryProvider WhereIsNotNull(string parameterName, object value)
            => Execute(() =>
            {
                parameterName = SqlServerValidation.SanitizeParameterName(parameterName);
                _Where.Add($"{parameterName} IS NOT NULL");
                AddParameter(parameterName, value);
            });

        public SqlServerQueryProvider WhereEquals(string columnName, string parameterName, object value)
            => WhereInner(columnName, parameterName, value, "=");

        public SqlServerQueryProvider WhereNotEquals(string columnName, string parameterName, object value)
            => WhereInner(columnName, parameterName, value, "<>");

        public SqlServerQueryProvider WhereGreaterThan(string columnName, string parameterName, object value)
            => WhereInner(columnName, parameterName, value, ">");

        public SqlServerQueryProvider WhereGreaterThanEquals(string columnName, string parameterName, object value)
            => WhereInner(columnName, parameterName, value, ">=");

        public SqlServerQueryProvider WhereLessThan(string columnName, string parameterName, object value)
            => WhereInner(columnName, parameterName, value, "<");

        public SqlServerQueryProvider WhereLessThanEquals(string columnName, string parameterName, object value)
            => WhereInner(columnName, parameterName, value, "<=");

        private SqlServerQueryProvider WhereInner(string columnName, string parameterName, object value, string op)
            => Execute(() =>
            {
                parameterName = SqlServerValidation.SanitizeParameterName(parameterName);
                columnName = FormatColumnName(SqlServerValidation.SanitizeColumnName(columnName));

                _Where.Add($"{columnName} {op} {parameterName}");
                AddParameter(parameterName, value);
            });

        public SqlServerQueryProvider AddParameter(string name, object value)
            => Execute(() => _Parameters.Add(name, value));
        
        public string GetNextParameterName(string parameterName)
            => $"{parameterName}{_CurrentParameterId++}";

        private SqlServerQueryProvider Execute(Action action)
        {
            action();
            return this;
        }

        private string FormatColumnName(string columnName)
        {
            if (_TargetTableAlias == null)
                return columnName;

            return _TargetTableAlias + "." + columnName;
        }

        public static SqlServerQueryProvider Empty => new();
        public static SqlServerQueryProvider WithParameters(Dictionary<string, object> parameters)
            => new()
            {
                _Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters))
            };
    }
}
