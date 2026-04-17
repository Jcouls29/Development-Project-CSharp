using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Sparcpoint;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Repositories;
using Sparcpoint.Inventory.SqlServer.Internal;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Inventory.SqlServer.Repositories
{
    /// <summary>
    /// EVAL: Category repository. Maintains optional hierarchies via CategoryCategories.
    /// </summary>
    public sealed class SqlCategoryRepository : ICategoryRepository
    {
        private readonly ISqlExecutor _executor;

        public SqlCategoryRepository(ISqlExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public Task<int> AddAsync(Category category, CancellationToken cancellationToken = default)
        {
            PreConditions.ParameterNotNull(category, nameof(category));
            PreConditions.StringNotNullOrWhitespace(category.Name, nameof(category.Name));

            if (category.Name.Length > 64) throw new ArgumentException("Name exceeds 64 chars", nameof(category));

            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                const string insertSql = @"
INSERT INTO [Instances].[Categories] ([Name], [Description])
OUTPUT INSERTED.[InstanceId]
VALUES (@Name, @Description);";

                var instanceId = await conn.ExecuteScalarAsync<int>(
                    new CommandDefinition(insertSql, new
                    {
                        Name = category.Name,
                        Description = category.Description ?? string.Empty
                    }, transaction: tx, cancellationToken: cancellationToken));

                if (category.Attributes != null && category.Attributes.Count > 0)
                {
                    await conn.ExecuteAsync(new CommandDefinition(@"
INSERT INTO [Instances].[CategoryAttributes] ([InstanceId], [Key], [Value])
VALUES (@InstanceId, @Key, @Value);",
                        category.Attributes
                            .Where(a => !string.IsNullOrWhiteSpace(a?.Key))
                            .Select(a => new { InstanceId = instanceId, a.Key, Value = a.Value ?? string.Empty })
                            .ToArray(),
                        transaction: tx, cancellationToken: cancellationToken));
                }

                if (category.ParentCategoryIds != null && category.ParentCategoryIds.Count > 0)
                {
                    // EVAL: CategoryCategories.InstanceId es el padre; CategoryInstanceId es el hijo.
                    await conn.ExecuteAsync(new CommandDefinition(@"
INSERT INTO [Instances].[CategoryCategories] ([InstanceId], [CategoryInstanceId])
VALUES (@ParentId, @ChildId);",
                        category.ParentCategoryIds.Distinct()
                            .Select(p => new { ParentId = p, ChildId = instanceId })
                            .ToArray(),
                        transaction: tx, cancellationToken: cancellationToken));
                }

                return instanceId;
            });
        }

        public Task<Category> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var row = await conn.QuerySingleOrDefaultAsync<CategoryRow>(new CommandDefinition(@"
SELECT [InstanceId], [Name], [Description], [CreatedTimestamp] FROM [Instances].[Categories]
WHERE [InstanceId] = @Id;",
                    new { Id = instanceId }, transaction: tx, cancellationToken: cancellationToken));

                if (row == null) return (Category)null;

                var category = MapToCategory(row);

                category.Attributes = (await conn.QueryAsync<CategoryAttributeRow>(new CommandDefinition(@"
SELECT [InstanceId], [Key], [Value] FROM [Instances].[CategoryAttributes] WHERE [InstanceId] = @Id;",
                    new { Id = instanceId }, transaction: tx, cancellationToken: cancellationToken)))
                    .Select(a => new CategoryAttribute(a.Key, a.Value)).ToList();

                category.ParentCategoryIds = (await conn.QueryAsync<int>(new CommandDefinition(@"
SELECT [InstanceId] FROM [Instances].[CategoryCategories] WHERE [CategoryInstanceId] = @Id;",
                    new { Id = instanceId }, transaction: tx, cancellationToken: cancellationToken))).ToList();

                return category;
            });
        }

        public Task<IReadOnlyList<Category>> ListAsync(CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var rows = (await conn.QueryAsync<CategoryRow>(new CommandDefinition(@"
SELECT [InstanceId], [Name], [Description], [CreatedTimestamp] FROM [Instances].[Categories]
ORDER BY [Name];",
                    transaction: tx, cancellationToken: cancellationToken))).ToList();

                return (IReadOnlyList<Category>)rows.Select(MapToCategory).ToList();
            });
        }

        private static Category MapToCategory(CategoryRow r) => new Category
        {
            InstanceId = r.InstanceId,
            Name = r.Name,
            Description = r.Description,
            CreatedTimestamp = r.CreatedTimestamp
        };
    }
}
