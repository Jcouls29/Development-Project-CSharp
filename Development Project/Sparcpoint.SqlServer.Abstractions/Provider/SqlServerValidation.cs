using System;
using System.Text.RegularExpressions;

namespace Sparcpoint.SqlServer.Abstractions
{
    internal static class SqlServerValidation
    {
        private static Regex SQL_OBJECT_NAME_PATTERN = new Regex(@"^\s*(([a-zA-Z][a-zA-Z0-9]*\.)?[a-zA-Z][a-zA-Z0-9_]*|(\[[a-zA-Z0-9_\s]+\]\.)?\[[a-zA-Z0-9_\s]+\])\s*$");
        public static string SanitizeColumnName(string columnName)
        {
            if (!SQL_OBJECT_NAME_PATTERN.IsMatch(columnName))
                throw new ArgumentException($"Invalid column name provided: '{columnName}'");

            return columnName.Trim();
        }

        public static string SanitizeTableName(string tableName)
        {
            if (!SQL_OBJECT_NAME_PATTERN.IsMatch(tableName))
                throw new ArgumentException($"Invalid table name provided: '{tableName}'");

            return tableName.Trim();
        }

        private static Regex PARAMETER_NAME_PATTERN = new Regex(@"^\s*\@?[a-zA-Z][a-zA-Z0-9_]*\s*$");
        public static string SanitizeParameterName(string parameterName)
        {
            if (!PARAMETER_NAME_PATTERN.IsMatch(parameterName))
                throw new ArgumentException($"Invalid parameter name provided: '{parameterName}'");

            parameterName = parameterName.Trim();

            if (!parameterName.StartsWith("@"))
                return "@" + parameterName;

            return parameterName;
        }
    }
}
