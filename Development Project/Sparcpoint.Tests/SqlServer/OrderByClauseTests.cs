using Sparcpoint.SqlServer.Abstractions;
using Xunit;
using static Sparcpoint.SqlServer.Abstractions.OrderByClause;

namespace Sparcpoint.Tests.SqlServer
{
    public class OrderByClauseTests
    {
        [Fact]
        public void ToString_WhenEmpty_ReturnsEmptyString()
        {
            var clause = new OrderByClause();

            Assert.Equal(string.Empty, clause.ToString());
        }

        [Fact]
        public void ToString_SingleAscending_ReturnsOrderByAsc()
        {
            var clause = new OrderByClause();
            clause.Add("Name", OrderByDirection.Ascending);

            Assert.Equal("ORDER BY Name ASC", clause.ToString());
        }

        [Fact]
        public void ToString_SingleDescending_ReturnsOrderByDesc()
        {
            var clause = new OrderByClause();
            clause.Add("CreatedTimestamp", OrderByDirection.Descending);

            Assert.Equal("ORDER BY CreatedTimestamp DESC", clause.ToString());
        }

        [Fact]
        public void ToString_MultipleColumns_CommaDelimited()
        {
            var clause = new OrderByClause();
            clause.Add("Name",             OrderByDirection.Ascending);
            clause.Add("CreatedTimestamp", OrderByDirection.Descending);

            var result = clause.ToString();

            Assert.StartsWith("ORDER BY ", result);
            Assert.Contains("Name ASC",             result);
            Assert.Contains("CreatedTimestamp DESC", result);
            Assert.Contains(",", result);
        }

        [Fact]
        public void Add_InvalidColumnName_ThrowsArgumentException()
        {
            var clause = new OrderByClause();

            // Spaces and special characters are not valid SQL object names
            Assert.Throws<System.ArgumentException>(
                () => clause.Add("'; DROP TABLE Products; --", OrderByDirection.Ascending));
        }
    }
}
