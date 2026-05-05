using Dapper;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.SqlServer
{
    public class SqlCategoryRepository : ICategoryRepository
    {
        private readonly ISqlExecutor _Executor;

        public SqlCategoryRepository(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<int> AddAsync(CategoryEntry category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Name is required.", nameof(category.Name));

            return await _Executor.ExecuteAsync(async (conn, trans) =>
            {
                int categoryId = await conn.QuerySingleAsync<int>(@"
                    INSERT INTO [Instances].[Categories] ([Name], [Description])
                    OUTPUT INSERTED.[InstanceId]
                    VALUES (@Name, @Description)",
                    new { category.Name, Description = category.Description ?? string.Empty }, trans);

                if (category.Attributes != null)
                {
                    foreach (var attr in category.Attributes)
                    {
                        await conn.ExecuteAsync(@"
                            INSERT INTO [Instances].[CategoryAttributes] ([InstanceId], [Key], [Value])
                            VALUES (@InstanceId, @Key, @Value)",
                            new { InstanceId = categoryId, attr.Key, attr.Value }, trans);
                    }
                }

                // CategoryCategories links this category as a child of each parent
                if (category.ParentCategoryIds != null)
                {
                    foreach (int parentId in category.ParentCategoryIds)
                    {
                        await conn.ExecuteAsync(@"
                            INSERT INTO [Instances].[CategoryCategories] ([InstanceId], [CategoryInstanceId])
                            VALUES (@InstanceId, @CategoryInstanceId)",
                            new { InstanceId = categoryId, CategoryInstanceId = parentId }, trans);
                    }
                }

                return categoryId;
            });
        }

        public async Task<CategoryEntry> GetByIdAsync(int instanceId)
        {
            return await _Executor.ExecuteAsync(async (conn, trans) =>
            {
                var record = await conn.QuerySingleOrDefaultAsync<CategoryDbRecord>(@"
                    SELECT [InstanceId], [Name], [Description], [CreatedTimestamp]
                    FROM [Instances].[Categories]
                    WHERE [InstanceId] = @InstanceId",
                    new { InstanceId = instanceId }, trans);

                if (record == null) return null;

                return await LoadSingleEntryAsync(conn, trans, record);
            });
        }

        public async Task<IEnumerable<CategoryEntry>> GetAllAsync()
        {
            return await _Executor.ExecuteAsync(async (conn, trans) =>
            {
                var records = (await conn.QueryAsync<CategoryDbRecord>(@"
                    SELECT [InstanceId], [Name], [Description], [CreatedTimestamp]
                    FROM [Instances].[Categories]
                    ORDER BY [Name] ASC",
                    transaction: trans)).ToList();

                if (!records.Any())
                    return Enumerable.Empty<CategoryEntry>();

                int[] ids = records.Select(r => r.InstanceId).ToArray();

                var attributes = await conn.QueryAsync<AttributeRecord>(@"
                    SELECT [InstanceId], [Key], [Value]
                    FROM [Instances].[CategoryAttributes]
                    WHERE [InstanceId] IN @Ids",
                    new { Ids = ids }, trans);

                var attrLookup = attributes
                    .GroupBy(a => a.InstanceId)
                    .ToDictionary(g => g.Key, g => g.ToDictionary(a => a.Key, a => a.Value));

                return records.Select(r => new CategoryEntry
                {
                    InstanceId = r.InstanceId,
                    Name = r.Name,
                    Description = r.Description,
                    CreatedTimestamp = r.CreatedTimestamp,
                    Attributes = attrLookup.TryGetValue(r.InstanceId, out var attrs)
                        ? attrs
                        : new Dictionary<string, string>()
                });
            });
        }

        private static async Task<CategoryEntry> LoadSingleEntryAsync(
            IDbConnection conn, IDbTransaction trans, CategoryDbRecord record)
        {
            var attributes = await conn.QueryAsync<AttributeRecord>(@"
                SELECT [InstanceId], [Key], [Value]
                FROM [Instances].[CategoryAttributes]
                WHERE [InstanceId] = @InstanceId",
                new { record.InstanceId }, trans);

            return new CategoryEntry
            {
                InstanceId = record.InstanceId,
                Name = record.Name,
                Description = record.Description,
                CreatedTimestamp = record.CreatedTimestamp,
                Attributes = attributes.ToDictionary(a => a.Key, a => a.Value)
            };
        }

        private class CategoryDbRecord
        {
            public int InstanceId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public DateTime CreatedTimestamp { get; set; }
        }

        private class AttributeRecord
        {
            public int InstanceId { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}
