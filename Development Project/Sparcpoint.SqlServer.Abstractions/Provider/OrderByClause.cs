using System.Collections;
using System.Collections.Generic;

namespace Sparcpoint.SqlServer.Abstractions
{
    public class OrderByClause : IEnumerable<string>
    {
        private readonly List<string> _Backer = new();

        public void Add(string columnName, OrderByDirection direction)
        {
            columnName = SqlServerValidation.SanitizeColumnName(columnName);
            _Backer.Add($"{columnName} {ToDirectionString(direction)}");
        }

        public override string ToString()
        {
            if (_Backer.Count == 0)
                return string.Empty;

            return "ORDER BY " + string.Join(", ", _Backer);
        }

        private string ToDirectionString(OrderByDirection direction)
            => direction == OrderByDirection.Ascending ? "ASC" : "DESC";

        public IEnumerator<string> GetEnumerator()
            => _Backer.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _Backer.GetEnumerator();

        public enum OrderByDirection : byte
        {
            Ascending = 0,
            Descending = 1
        }
    }
}
