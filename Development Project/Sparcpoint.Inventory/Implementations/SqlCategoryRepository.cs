using Dapper;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Extensions;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Implementations
{
    // EVAL: Categories support arbitrary-depth hierarchies via the existing CategoryCategories
    // junction table. A child category can have multiple parents (DAG), which is more flexible than
    // a strict tree and matches what the schema allows.
    public class SqlCategoryRepository : ICategoryRepository
    {
        private readonly ISqlExecutor _Executor;

        public SqlCategoryRepository(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public Task<int> CreateAsync(CreateCategoryRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));
            PreConditions.StringNotNullOrWhitespace(request.Name, nameof(request.Name));

            return _Executor.ExecuteAsync<int>(async (conn, trans) =>
            {
                var categoryId = await conn.QuerySingleAsync<int>(@"
                    INSERT INTO [Instances].[Categories] ([Name], [Description])
                    VALUES (@Name, @Description);
                    SELECT SCOPE_IDENTITY();",
                    new
                    {
                        Name = request.Name.Truncate(64),
                        Description = (request.Description ?? string.Empty).Truncate(256)
                    },
                    trans);

                if (request.Attributes != null && request.Attributes.Count > 0)
                {
                    var attrTable = new DataTable();
                    attrTable.Columns.Add("Key", typeof(string));
                    attrTable.Columns.Add("Value", typeof(string));

                    foreach (var kvp in request.Attributes)
                        attrTable.Rows.Add(kvp.Key.Truncate(64), kvp.Value.Truncate(512));

                    await conn.ExecuteAsync(@"
                        INSERT INTO [Instances].[CategoryAttributes] ([InstanceId], [Key], [Value])
                        SELECT @CategoryId, [Key], [Value] FROM @Attributes",
                        new { CategoryId = categoryId, Attributes = attrTable.AsTableValuedParameter("dbo.CustomAttributeList") },
                        trans);
                }

                if (request.ParentCategoryIds != null && request.ParentCategoryIds.Count > 0)
                {
                    var parentTable = new DataTable();
                    parentTable.Columns.Add("Value", typeof(int));

                    foreach (var parentId in request.ParentCategoryIds)
                        parentTable.Rows.Add(parentId);

                    await conn.ExecuteAsync(@"
                        INSERT INTO [Instances].[CategoryCategories] ([InstanceId], [CategoryInstanceId])
                        SELECT @CategoryId, [Value] FROM @ParentIds",
                        new { CategoryId = categoryId, ParentIds = parentTable.AsTableValuedParameter("dbo.IntegerList") },
                        trans);
                }

                return categoryId;
            });
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        {
            return _Executor.ExecuteAsync<IEnumerable<Category>>(async (conn, trans) =>
            {
                var rows = (await conn.QueryAsync<CategoryRow>(@"
                    SELECT [InstanceId], [Name], [Description], [CreatedTimestamp]
                    FROM [Instances].[Categories]
                    ORDER BY [Name]",
                    transaction: trans)).ToList();

                return await HydrateAsync(conn, trans, rows);
            });
        }

        public Task<Category> GetByIdAsync(int categoryId)
        {
            return _Executor.ExecuteAsync<Category>(async (conn, trans) =>
            {
                var row = await conn.QuerySingleOrDefaultAsync<CategoryRow>(@"
                    SELECT [InstanceId], [Name], [Description], [CreatedTimestamp]
                    FROM [Instances].[Categories]
                    WHERE [InstanceId] = @CategoryId",
                    new { CategoryId = categoryId }, trans);

                if (row == null) return null;

                var results = await HydrateAsync(conn, trans, new[] { row });
                return results.FirstOrDefault();
            });
        }

        private async Task<IEnumerable<Category>> HydrateAsync(IDbConnection conn, IDbTransaction trans, IList<CategoryRow> rows)
        {
            if (rows.Count == 0) return Enumerable.Empty<Category>();

            var ids = rows.Select(r => r.InstanceId).ToList();
            var idTable = new DataTable();
            idTable.Columns.Add("Value", typeof(int));
            foreach (var id in ids) idTable.Rows.Add(id);

            var attributes = (await conn.QueryAsync<CategoryAttributeRow>(@"
                SELECT [InstanceId], [Key], [Value]
                FROM [Instances].[CategoryAttributes]
                WHERE [InstanceId] IN (SELECT [Value] FROM @Ids)",
                new { Ids = idTable.AsTableValuedParameter("dbo.IntegerList") }, trans)).ToLookup(a => a.InstanceId);

            var parents = (await conn.QueryAsync<CategoryParentRow>(@"
                SELECT [InstanceId], [CategoryInstanceId]
                FROM [Instances].[CategoryCategories]
                WHERE [InstanceId] IN (SELECT [Value] FROM @Ids)",
                new { Ids = idTable.AsTableValuedParameter("dbo.IntegerList") }, trans)).ToLookup(p => p.InstanceId);

            return rows.Select(r => new Category
            {
                CategoryId = r.InstanceId,
                Name = r.Name,
                Description = r.Description,
                CreatedTimestamp = r.CreatedTimestamp,
                Attributes = attributes[r.InstanceId].ToDictionary(a => a.Key, a => a.Value),
                ParentCategoryIds = parents[r.InstanceId].Select(p => p.CategoryInstanceId).ToList()
            });
        }

        private class CategoryRow
        {
            public int InstanceId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public DateTime CreatedTimestamp { get; set; }
        }

        private class CategoryAttributeRow
        {
            public int InstanceId { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private class CategoryParentRow
        {
            public int InstanceId { get; set; }
            public int CategoryInstanceId { get; set; }
        }
    }
}
