using Dapper;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System.Data;

namespace Sparcpoint.Inventory.Repositories.SqlServer
{
    /// <summary>
    /// EVAL: SQL Server implementation of ICategoryRepository using ISqlExecutor and Dapper.
    /// Supports hierarchical categories via [Instances].[CategoryCategories] junction table.
    /// </summary>
    public class SqlCategoryRepository : ICategoryRepository
    {
        private readonly ISqlExecutor _SqlExecutor;

        public SqlCategoryRepository(ISqlExecutor sqlExecutor)
        {
            _SqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public async Task<Category> CreateAsync(Category category)
        {
            return await _SqlExecutor.ExecuteAsync<Category>(async (conn, trans) =>
            {
                const string insertSql = @"
                    INSERT INTO [Instances].[Categories] ([Name], [Description], [CreatedTimestamp])
                    VALUES (@Name, @Description, @CreatedTimestamp);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                category.CreatedTimestamp = DateTime.UtcNow;

                var instanceId = await conn.QuerySingleAsync<int>(insertSql, new
                {
                    category.Name,
                    category.Description,
                    category.CreatedTimestamp
                }, trans);

                category.InstanceId = instanceId;

                // EVAL: Insert category attributes as key-value pairs
                if (category.Attributes?.Count > 0)
                {
                    const string insertAttrSql = @"
                        INSERT INTO [Instances].[CategoryAttributes] ([InstanceId], [Key], [Value])
                        VALUES (@InstanceId, @Key, @Value);";

                    foreach (var attr in category.Attributes)
                    {
                        await conn.ExecuteAsync(insertAttrSql, new
                        {
                            InstanceId = instanceId,
                            Key = attr.Key,
                            Value = attr.Value
                        }, trans);
                    }
                }

                // EVAL: Link to parent categories via [CategoryCategories] junction table
                // InstanceId = child, CategoryInstanceId = parent (the category this one belongs to)
                if (category.ParentCategoryIds?.Count > 0)
                {
                    const string insertParentSql = @"
                        INSERT INTO [Instances].[CategoryCategories] ([InstanceId], [CategoryInstanceId])
                        VALUES (@InstanceId, @CategoryInstanceId);";

                    foreach (var parentId in category.ParentCategoryIds)
                    {
                        await conn.ExecuteAsync(insertParentSql, new
                        {
                            InstanceId = instanceId,
                            CategoryInstanceId = parentId
                        }, trans);
                    }
                }

                return category;
            });
        }

        public async Task<Category?> GetByIdAsync(int instanceId)
        {
            return await _SqlExecutor.ExecuteAsync<Category?>(async (conn, trans) =>
            {
                const string categorySql = @"
                    SELECT [InstanceId], [Name], [Description], [CreatedTimestamp]
                    FROM [Instances].[Categories]
                    WHERE [InstanceId] = @InstanceId;";

                var category = await conn.QuerySingleOrDefaultAsync<Category>(categorySql, new { InstanceId = instanceId }, trans);
                if (category == null) return null;

                // Load attributes
                const string attrSql = @"
                    SELECT [Key], [Value]
                    FROM [Instances].[CategoryAttributes]
                    WHERE [InstanceId] = @InstanceId;";

                var attrs = await conn.QueryAsync(attrSql, new { InstanceId = instanceId }, trans);
                category.Attributes = new Dictionary<string, string>();
                foreach (var attr in attrs)
                {
                    category.Attributes[attr.Key] = attr.Value;
                }

                // Load parent category associations
                const string parentSql = @"
                    SELECT [CategoryInstanceId]
                    FROM [Instances].[CategoryCategories]
                    WHERE [InstanceId] = @InstanceId;";

                var parentIds = await conn.QueryAsync<int>(parentSql, new { InstanceId = instanceId }, trans);
                category.ParentCategoryIds = parentIds.ToList();

                return category;
            });
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _SqlExecutor.ExecuteAsync<IEnumerable<Category>>(async (conn, trans) =>
            {
                const string sql = @"
                    SELECT [InstanceId], [Name], [Description], [CreatedTimestamp]
                    FROM [Instances].[Categories]
                    ORDER BY [Name];";

                var categories = (await conn.QueryAsync<Category>(sql, transaction: trans)).ToList();

                if (categories.Count > 0)
                {
                    var categoryIds = categories.Select(c => c.InstanceId).ToList();

                    const string attrSql = @"
                        SELECT [InstanceId], [Key], [Value]
                        FROM [Instances].[CategoryAttributes]
                        WHERE [InstanceId] IN @CategoryIds;";

                    var allAttrs = await conn.QueryAsync(attrSql, new { CategoryIds = categoryIds }, trans);
                    var attrLookup = allAttrs.GroupBy(a => (int)a.InstanceId)
                        .ToDictionary(g => g.Key, g => g.ToDictionary(a => (string)a.Key, a => (string)a.Value));

                    const string parentSql = @"
                        SELECT [InstanceId], [CategoryInstanceId]
                        FROM [Instances].[CategoryCategories]
                        WHERE [InstanceId] IN @CategoryIds;";

                    var allParents = await conn.QueryAsync(parentSql, new { CategoryIds = categoryIds }, trans);
                    var parentLookup = allParents.GroupBy(p => (int)p.InstanceId)
                        .ToDictionary(g => g.Key, g => g.Select(p => (int)p.CategoryInstanceId).ToList());

                    foreach (var category in categories)
                    {
                        category.Attributes = attrLookup.TryGetValue(category.InstanceId, out var attrs) ? attrs : new Dictionary<string, string>();
                        category.ParentCategoryIds = parentLookup.TryGetValue(category.InstanceId, out var parents) ? parents : new List<int>();
                    }
                }

                return categories;
            });
        }

        /// <summary>
        /// EVAL: Returns child categories for a given parent, enabling hierarchical navigation.
        /// Uses [CategoryCategories] where CategoryInstanceId = parent to find children.
        /// </summary>
        public async Task<IEnumerable<Category>> GetChildrenAsync(int parentCategoryId)
        {
            return await _SqlExecutor.ExecuteAsync<IEnumerable<Category>>(async (conn, trans) =>
            {
                const string sql = @"
                    SELECT c.[InstanceId], c.[Name], c.[Description], c.[CreatedTimestamp]
                    FROM [Instances].[Categories] c
                    INNER JOIN [Instances].[CategoryCategories] cc
                        ON c.[InstanceId] = cc.[InstanceId]
                    WHERE cc.[CategoryInstanceId] = @ParentCategoryId
                    ORDER BY c.[Name];";

                return await conn.QueryAsync<Category>(sql, new { ParentCategoryId = parentCategoryId }, trans);
            });
        }
    }
}
