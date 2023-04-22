using System.Collections;
using System.Collections.Generic;

namespace Sparcpoint.SqlServer.Abstractions
{
    public class JoinTables : IEnumerable<string>
    {
        private readonly List<string> _Backer = new();

        public void Add(string clause)
            => _Backer.Add(clause);

        public void Add(string tableName, string abbv, string onClause)
            => _Backer.Add($"JOIN {tableName} {abbv} ON {onClause}");

        public override string ToString()
        {
            if (_Backer.Count == 0)
                return "";

            return string.Join("\n", _Backer);
        }

        public IEnumerator<string> GetEnumerator()
            => _Backer.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _Backer.GetEnumerator();
    }
}
