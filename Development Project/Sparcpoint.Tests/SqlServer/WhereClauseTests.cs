using Sparcpoint.SqlServer.Abstractions;
using Xunit;

namespace Sparcpoint.Tests.SqlServer
{
    public class WhereClauseTests
    {
        [Fact]
        public void ToString_WhenEmpty_ReturnsEmptyString()
        {
            var clause = new WhereClause();

            Assert.Equal(string.Empty, clause.ToString());
        }

        [Fact]
        public void ToString_SingleCondition_ReturnsPrefixedClause()
        {
            var clause = new WhereClause();
            clause.Add("p.[Name] = @name");

            Assert.Equal("WHERE p.[Name] = @name", clause.ToString());
        }

        [Fact]
        public void ToString_MultipleConditions_JoinsWithAnd()
        {
            var clause = new WhereClause();
            clause.Add("p.[Name] = @name");
            clause.Add("p.[InstanceId] = @id");

            var result = clause.ToString();

            Assert.StartsWith("WHERE ", result);
            Assert.Contains("p.[Name] = @name", result);
            Assert.Contains("p.[InstanceId] = @id", result);
            Assert.Contains(" AND ", result);
        }

        [Fact]
        public void IsEnumerable_CanIterateClauses()
        {
            var clause = new WhereClause();
            clause.Add("A = @a");
            clause.Add("B = @b");

            int count = 0;
            foreach (var _ in clause) count++;

            Assert.Equal(2, count);
        }
    }
}
