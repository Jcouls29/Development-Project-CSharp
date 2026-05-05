using System;
using System.Collections.Generic;
using Sparcpoint.SqlServer.Abstractions;
using Xunit;

namespace Sparcpoint.Tests.SqlServer
{
    public class SqlServerQueryProviderTests
    {
        // ── Factory ───────────────────────────────────────────────────────────

        [Fact]
        public void Empty_ReturnsProviderWithNoClauses()
        {
            var qp = SqlServerQueryProvider.Empty;

            Assert.Equal(string.Empty, qp.WhereClause);
            Assert.Equal(string.Empty, qp.OrderByClause);
            Assert.Equal(string.Empty, qp.JoinClause);
            Assert.Empty(qp.Parameters);
        }

        [Fact]
        public void WithParameters_NullDictionary_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => SqlServerQueryProvider.WithParameters(null!));
        }

        [Fact]
        public void WithParameters_ValidDictionary_SetsParameters()
        {
            var initial = new Dictionary<string, object> { ["@id"] = 1 };

            var qp = SqlServerQueryProvider.WithParameters(initial);

            Assert.Single(qp.Parameters);
        }

        // ── Where ─────────────────────────────────────────────────────────────

        [Fact]
        public void Where_SingleClause_AppearsInWhereClause()
        {
            var qp = SqlServerQueryProvider.Empty
                .Where("p.[Name] LIKE @name");

            Assert.Contains("p.[Name] LIKE @name", qp.WhereClause);
            Assert.StartsWith("WHERE", qp.WhereClause);
        }

        [Fact]
        public void WhereEquals_AddsConditionAndParameter()
        {
            var qp = SqlServerQueryProvider.Empty
                .WhereEquals("Name", "name", "Widget");

            Assert.Contains("Name = @name", qp.WhereClause);
            Assert.True(qp.Parameters.ContainsKey("@name"));
            Assert.Equal("Widget", qp.Parameters["@name"]);
        }

        [Fact]
        public void WhereNotEquals_AddsNotEqualsCondition()
        {
            var qp = SqlServerQueryProvider.Empty
                .WhereNotEquals("Status", "status", "Deleted");

            Assert.Contains("<>", qp.WhereClause);
        }

        [Fact]
        public void WhereGreaterThan_AddsGreaterThanCondition()
        {
            var qp = SqlServerQueryProvider.Empty
                .WhereGreaterThan("Quantity", "qty", 0);

            Assert.Contains(">", qp.WhereClause);
        }

        [Fact]
        public void WhereIsNull_AddsIsNullCondition()
        {
            var qp = SqlServerQueryProvider.Empty
                .WhereIsNull("CompletedTimestamp");

            Assert.Contains("IS NULL", qp.WhereClause);
        }

        [Fact]
        public void WhereIsNotNull_AddsIsNotNullCondition()
        {
            var qp = SqlServerQueryProvider.Empty
                .WhereIsNotNull("CompletedTimestamp");

            Assert.Contains("IS NOT NULL", qp.WhereClause);
        }

        [Fact]
        public void MultipleWhereClauses_JoinedWithAnd()
        {
            var qp = SqlServerQueryProvider.Empty
                .WhereEquals("Name",   "name",   "Widget")
                .WhereEquals("Status", "status", "Active");

            Assert.Contains(" AND ", qp.WhereClause);
        }

        // ── OrderBy ───────────────────────────────────────────────────────────

        [Fact]
        public void OrderByAscending_AppearsInOrderByClause()
        {
            var qp = SqlServerQueryProvider.Empty
                .OrderByAscending("Name");

            Assert.Contains("Name ASC", qp.OrderByClause);
            Assert.StartsWith("ORDER BY", qp.OrderByClause);
        }

        [Fact]
        public void OrderByDescending_AppearsInOrderByClause()
        {
            var qp = SqlServerQueryProvider.Empty
                .OrderByDescending("CreatedTimestamp");

            Assert.Contains("CreatedTimestamp DESC", qp.OrderByClause);
        }

        // ── Join ──────────────────────────────────────────────────────────────

        [Fact]
        public void Join_AppearsInJoinClause()
        {
            var qp = SqlServerQueryProvider.Empty
                .Join("[Instances].[ProductAttributes]", "pa", "pa.[InstanceId] = p.[InstanceId]");

            Assert.Contains("JOIN", qp.JoinClause);
            Assert.Contains("[Instances].[ProductAttributes]", qp.JoinClause);
        }

        // ── Parameters ────────────────────────────────────────────────────────

        [Fact]
        public void AddParameter_StoresValue()
        {
            var qp = SqlServerQueryProvider.Empty
                .AddParameter("@count", 42);

            Assert.True(qp.Parameters.ContainsKey("@count"));
            Assert.Equal(42, qp.Parameters["@count"]);
        }

        [Fact]
        public void Parameters_ReturnsDefensiveCopy()
        {
            var qp = SqlServerQueryProvider.Empty.AddParameter("@x", 1);
            var params1 = qp.Parameters;
            var params2 = qp.Parameters;

            // Modifying one copy should not affect the other or the provider
            params1["@extra"] = 99;

            Assert.False(params2.ContainsKey("@extra"));
            Assert.False(qp.Parameters.ContainsKey("@extra"));
        }

        [Fact]
        public void GetNextParameterName_IncrementsSuffix()
        {
            var qp = SqlServerQueryProvider.Empty;

            var p0 = qp.GetNextParameterName("attr");
            var p1 = qp.GetNextParameterName("attr");

            Assert.NotEqual(p0, p1);
        }

        // ── TableAlias ────────────────────────────────────────────────────────

        [Fact]
        public void SetTargetTableAlias_PrefixesColumnsInWhere()
        {
            var qp = SqlServerQueryProvider.Empty
                .SetTargetTableAlias("p")
                .WhereEquals("Name", "name", "Widget");

            Assert.Contains("p.Name", qp.WhereClause);
        }

        [Fact]
        public void ClearTargetTableAlias_StopsColumnPrefixing()
        {
            var qp = SqlServerQueryProvider.Empty
                .SetTargetTableAlias("p")
                .ClearTargetTableAlias()
                .WhereEquals("Name", "name", "Widget");

            // After clearing, column should not be prefixed
            Assert.DoesNotContain("p.Name", qp.WhereClause);
        }

        // ── Fluent / Immutability ─────────────────────────────────────────────

        [Fact]
        public void FluentMethods_ReturnSameInstance()
        {
            var qp = SqlServerQueryProvider.Empty;
            var returned = qp.Where("1 = 1");

            Assert.Same(qp, returned);
        }

        // ── Input validation ──────────────────────────────────────────────────

        [Fact]
        public void WhereEquals_InvalidColumnName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                SqlServerQueryProvider.Empty.WhereEquals("'; DROP TABLE--", "p", "x"));
        }
    }
}
