using Sparcpoint.SqlServer.Abstractions;
using Xunit;

namespace Sparcpoint.Tests.SqlServer
{
    public class JoinTablesTests
    {
        [Fact]
        public void ToString_WhenEmpty_ReturnsEmptyString()
        {
            var joins = new JoinTables();

            Assert.Equal(string.Empty, joins.ToString());
        }

        [Fact]
        public void Add_TableNameAbvAndOnClause_FormatsJoinCorrectly()
        {
            var joins = new JoinTables();
            joins.Add("[Instances].[ProductAttributes]", "pa", "pa.[InstanceId] = p.[InstanceId]");

            Assert.Contains("JOIN [Instances].[ProductAttributes] pa ON pa.[InstanceId] = p.[InstanceId]",
                joins.ToString());
        }

        [Fact]
        public void Add_RawClause_AppendsAsIs()
        {
            var joins = new JoinTables();
            joins.Add("INNER JOIN SomeTable t ON t.Id = p.Id");

            Assert.Contains("INNER JOIN SomeTable t ON t.Id = p.Id", joins.ToString());
        }

        [Fact]
        public void ToString_MultipleJoins_SeparatedByNewlines()
        {
            var joins = new JoinTables();
            joins.Add("TableA", "a", "a.Id = p.Id");
            joins.Add("TableB", "b", "b.Id = p.Id");

            var result = joins.ToString();
            var lines  = result.Split('\n');

            Assert.Equal(2, lines.Length);
        }

        [Fact]
        public void IsEnumerable_CanIterateJoins()
        {
            var joins = new JoinTables();
            joins.Add("T1", "t1", "t1.Id = p.Id");
            joins.Add("T2", "t2", "t2.Id = p.Id");

            int count = 0;
            foreach (var _ in joins) count++;

            Assert.Equal(2, count);
        }
    }
}
