using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Sparcpoint.Inventory.Models.Domain;
using Sparcpoint.Inventory.Repositories.Interfaces;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Inventory.Repositories.Implementations
{
    /// <summary>
    /// Category repository implementation.
    /// EVAL: Handles hierarchical category relationships via CategoryCategories table.
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ISqlExecutor _sqlExecutor;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(ISqlExecutor sqlExecutor, ILogger<CategoryRepository> logger)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            // EVAL: Similar pattern to ProductRepository - uses TVPs for bulk attribute/parent inserts
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                const string insertCategorySql = @"
                    INSERT INTO [Instances].[Categories] 
                    ([Name], [Description])
                    VALUES 
                    (@Name, @Description);
                    
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var categoryId = await conn.QuerySingleAsync<int>(
                    insertCategorySql,
                    new
                    {
                        category.Name,
                        category.Description
                    },
                    trans);

                category.InstanceId = categoryId;

                // Insert attributes
                if (category.Attributes != null && category.Attributes.Any())
                {
                    var attributeTable = CreateAttributeTable(category.Attributes);
                    
                    const string insertAttributesSql = @"
                        INSERT INTO [Instances].[CategoryAttributes] ([InstanceId], [Key], [Value])
                        SELECT @CategoryId, [Key], [Value]
                        FROM @Attributes";

                    await conn.ExecuteAsync(
                        insertAttributesSql,
                        new { CategoryId = categoryId, Attributes = attributeTable.AsTableValuedParameter("[dbo].[CustomAttributeList]") },
                        trans);
                }

                // Insert parent category relationships
                if (category.ParentCategoryIds != null && category.ParentCategoryIds.Any())
                {
                    var parentTable = CreateIntegerTable(category.ParentCategoryIds);
                    
                    const string insertParentsSql = @"
                        INSERT INTO [Instances].[CategoryCategories] ([InstanceId], [CategoryInstanceId])
                        SELECT @CategoryId, [Value]
                        FROM @Parents";

                    await conn.ExecuteAsync(
                        insertParentsSql,
                        new { CategoryId = categoryId, Parents = parentTable.AsTableValuedParameter("[dbo].[IntegerList]") },
                        trans);
                }

                _logger.LogInformation("Created category {CategoryId} with {AttributeCount} attributes and {ParentCount} parents",
                    categoryId, category.Attributes?.Count ?? 0, category.ParentCategoryIds?.Count ?? 0);

                
                return category;
            });
        }

        public async Task<Category?> GetCategoryByIdAsync(int instanceId)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                const string categorySql = "SELECT * FROM [Instances].[Categories] WHERE [InstanceId] = @InstanceId";
                const string attributesSql = "SELECT [Key], [Value] FROM [Instances].[CategoryAttributes] WHERE [InstanceId] = @InstanceId";
                const string parentsSql = "SELECT [CategoryInstanceId] FROM [Instances].[CategoryCategories] WHERE [InstanceId] = @InstanceId";

                var category = await conn.QuerySingleOrDefaultAsync<CategoryDto>(categorySql, new { InstanceId = instanceId }, trans);
                if (category == null) return null;

                var attributes = await conn.QueryAsync<AttributeDto>(attributesSql, new { InstanceId = instanceId }, trans);
                var parents = await conn.QueryAsync<int>(parentsSql, new { InstanceId = instanceId }, trans);

                return MapToCategory(category, attributes, parents);
            });
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                const string sql = "SELECT * FROM [Instances].[Categories] ORDER BY [Name]";
                var categoryDtos = await conn.QueryAsync<CategoryDto>(sql, transaction: trans);

                var categories = new List<Category>();
                foreach (var dto in categoryDtos)
                {
                    var attributes = await conn.QueryAsync<AttributeDto>(
                        "SELECT [Key], [Value] FROM [Instances].[CategoryAttributes] WHERE [InstanceId] = @InstanceId",
                        new { dto.InstanceId },
                        trans);

                    var parents = await conn.QueryAsync<int>(
                        "SELECT [CategoryInstanceId] FROM [Instances].[CategoryCategories] WHERE [InstanceId] = @InstanceId",
                        new { dto.InstanceId },
                        trans);

                    categories.Add(MapToCategory(dto, attributes, parents));
                }

                return categories;
            });
        }

        public async Task<List<Category>> GetCategoriesByIdsAsync(List<int> categoryIds)
        {
            if (categoryIds == null || !categoryIds.Any())
                return new List<Category>();

            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var idsTable = CreateIntegerTable(categoryIds);

                const string sql = @"
                    SELECT c.* 
                    FROM [Instances].[Categories] c
                    INNER JOIN @Ids ids ON c.[InstanceId] = ids.[Value]";

                var categoryDtos = await conn.QueryAsync<CategoryDto>(
                    sql,
                    new { Ids = idsTable.AsTableValuedParameter("[dbo].[IntegerList]") },
                    trans);

                var categories = new List<Category>();
                foreach (var dto in categoryDtos)
                {
                    var attributes = await conn.QueryAsync<AttributeDto>(
                        "SELECT [Key], [Value] FROM [Instances].[CategoryAttributes] WHERE [InstanceId] = @InstanceId",
                        new { dto.InstanceId },
                        trans);

                    var parents = await conn.QueryAsync<int>(
                        "SELECT [CategoryInstanceId] FROM [Instances].[CategoryCategories] WHERE [InstanceId] = @InstanceId",
                        new { dto.InstanceId },
                        trans);

                    categories.Add(MapToCategory(dto, attributes, parents));
                }

                return categories;
            });
        }

        #region Helper Methods

        private DataTable CreateAttributeTable(Dictionary<string, string> attributes)
        {
            var table = new DataTable();
            table.Columns.Add("Key", typeof(string));
            table.Columns.Add("Value", typeof(string));

            foreach (var attr in attributes)
            {
                table.Rows.Add(attr.Key, attr.Value);
            }

            return table;
        }

        private DataTable CreateIntegerTable(List<int> values)
        {
            var table = new DataTable();
            table.Columns.Add("Value", typeof(int));

            foreach (var value in values)
            {
                table.Rows.Add(value);
            }

            return table;
        }

        private Category MapToCategory(CategoryDto dto, IEnumerable<AttributeDto> attributes, IEnumerable<int> parentIds)
        {
            return new Category
            {
                InstanceId = dto.InstanceId,
                Name = dto.Name,
                Description = dto.Description,
                Attributes = attributes?.ToDictionary(a => a.Key, a => a.Value) ?? new Dictionary<string, string>(),
                ParentCategoryIds = parentIds?.ToList() ?? new List<int>(),
                CreatedTimestamp = dto.CreatedTimestamp
            };
        }

        #endregion

        #region DTOs

        private class CategoryDto
        {
            public int InstanceId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public DateTime CreatedTimestamp { get; set; }
        }

        private class AttributeDto
        {
            public string Key { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
        }

        #endregion
    }
}