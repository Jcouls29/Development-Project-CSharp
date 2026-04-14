using Dapper;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Repositories.Sql
{
    public sealed class SqlCategoryRepository : ICategoryRepository
    {
        private readonly ISqlExecutor _executor;

        public SqlCategoryRepository(ISqlExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public Task<int> AddAsync(Category category, CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                var id = await conn.ExecuteScalarAsync<int>(Queries.InsertCategory, new { category.Name, category.Description }, tx);
                foreach (var a in category.Attributes ?? Array.Empty<ProductAttribute>())
                    await conn.ExecuteAsync(Queries.InsertCategoryAttribute, new { InstanceId = id, a.Key, a.Value }, tx);
                foreach (var parentId in category.ParentCategoryIds ?? Array.Empty<int>())
                    await conn.ExecuteAsync(Queries.InsertCategoryParent, new { InstanceId = id, ParentInstanceId = parentId }, tx);
                return id;
            });
        }

        public Task<Category?> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync<Category?>(async (conn, tx) =>
            {
                var row = await conn.QuerySingleOrDefaultAsync<CategoryRow>(Queries.SelectCategoryById, new { InstanceId = instanceId }, tx);
                if (row is null) return null;
                var attrs = (await conn.QueryAsync<ProductAttribute>(Queries.SelectCategoryAttributes, new { InstanceId = instanceId }, tx)).ToArray();
                var parents = (await conn.QueryAsync<int>(Queries.SelectCategoryParents, new { InstanceId = instanceId }, tx)).ToArray();
                return Materialize(row, attrs, parents);
            });
        }

        public Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync<IReadOnlyList<Category>>(async (conn, tx) =>
            {
                var rows = (await conn.QueryAsync<CategoryRow>(Queries.SelectAllCategories, transaction: tx)).ToArray();
                return rows.Select(r => Materialize(r, Array.Empty<ProductAttribute>(), Array.Empty<int>())).ToArray();
            });
        }

        public Task<bool> UpdateAsync(Category category, CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                var affected = await conn.ExecuteAsync(Queries.UpdateCategory, new { category.InstanceId, category.Name, category.Description }, tx);
                if (affected == 0) return false;

                // EVAL: Full-replace of child collections keeps update logic simple and predictable.
                await conn.ExecuteAsync(Queries.DeleteCategoryAttributes, new { category.InstanceId }, tx);
                await conn.ExecuteAsync(Queries.DeleteCategoryParents, new { category.InstanceId }, tx);

                foreach (var a in category.Attributes ?? Array.Empty<ProductAttribute>())
                    await conn.ExecuteAsync(Queries.InsertCategoryAttribute, new { category.InstanceId, a.Key, a.Value }, tx);
                foreach (var parentId in category.ParentCategoryIds ?? Array.Empty<int>())
                    await conn.ExecuteAsync(Queries.InsertCategoryParent, new { category.InstanceId, ParentInstanceId = parentId }, tx);
                return true;
            });
        }

        public Task<bool> DeleteAsync(int instanceId, CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                var affected = await conn.ExecuteAsync(Queries.DeleteCategory, new { InstanceId = instanceId }, tx);
                return affected > 0;
            });
        }

        public Task<IReadOnlyList<Category>> GetDescendantsAsync(int rootInstanceId, CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync<IReadOnlyList<Category>>(async (conn, tx) =>
            {
                var rows = (await conn.QueryAsync<CategoryRow>(Queries.SelectCategoryDescendants, new { RootId = rootInstanceId }, tx)).ToArray();
                return rows.Select(r => Materialize(r, Array.Empty<ProductAttribute>(), Array.Empty<int>())).ToArray();
            });
        }

        private static Category Materialize(CategoryRow row, IReadOnlyList<ProductAttribute> attrs, IReadOnlyList<int> parents) => new()
        {
            InstanceId = row.InstanceId,
            Name = row.Name,
            Description = row.Description,
            CreatedTimestamp = row.CreatedTimestamp,
            Attributes = attrs,
            ParentCategoryIds = parents,
        };

        private sealed class CategoryRow
        {
            public int InstanceId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public DateTime CreatedTimestamp { get; set; }
        }
    }
}
