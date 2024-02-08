using System.Collections;
using System.Collections.Generic;

namespace Sparcpoint.SqlServer.Abstractions
{
    public class WhereClause : IEnumerable<string>
    {
        private readonly List<string> _Backer = new List<string>();

        public void Add(string clause)
            => _Backer.Add(clause);

        public override string ToString()
        {
            if (_Backer.Count == 0)
                return string.Empty;

            return "WHERE " + string.Join(" AND ", _Backer);
        }

        public IEnumerator<string> GetEnumerator()
            => _Backer.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _Backer.GetEnumerator();
    }
}
