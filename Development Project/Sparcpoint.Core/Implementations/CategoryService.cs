using Dapper.Contrib.Extensions;
using Sparcpoint.Application.Abstracts;
using Sparcpoint.Core.Entities;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Implementations
{
    public class Category : ICategoryService
    {
        private readonly SqlServerQueryProvider _sqlServerQueryProvider;
        private readonly IQueryService _queryService;
        private readonly ISqlExecutor _sqlExecutor;
        private readonly string TableName = "[Instances].[Categories]";

        public Category(SqlServerQueryProvider sqlServerQueryProvider, IQueryService queryService, ISqlExecutor sqlExecutor)
        {
            _sqlServerQueryProvider = sqlServerQueryProvider;
            _queryService = queryService;
            _sqlExecutor = sqlExecutor;
        }
        public Task AddAttributeToCategory(int categoryId, List<CategoryAttribute> attributes)
        {
            throw new NotImplementedException();
        }

        public async Task AddAttributeToCategoryAsync(int categoryId, List<CategoryAttribute> attributes)
        {
            await _sqlExecutor.ExecuteAsync(async (connection, transaction) => {

                if (attributes.Count > 0)
                {
                    foreach (var item in attributes)
                    {
                        item.InstanceId = categoryId;
                        await connection.InsertAsync(item, transaction);
                    }
                }
            });
        }

        public Task AddCategoryToCategories(int categoryId, List<CategoryOfCategory> categories)
        {
            throw new NotImplementedException();
        }

        public async Task AddCategoryToCategoriesAsync(int categoryId, List<CategoryOfCategory> categories)
        {
            await _sqlExecutor.ExecuteAsync(async (connection, transaction) => {

                if (categories.Count > 0)
                {
                    foreach (var item in categories)
                    {
                        item.InstanceId = categoryId;
                        await connection.InsertAsync(item, transaction);
                    }
                }
            });
        }
        public Task AddCategoryToCategory(int categoryId, CategoryOfCategory parentCategory)
        {
            throw new NotImplementedException();
        }
        public async Task AddCategoryToCategoryAsync(int categoryId, CategoryOfCategory parentCategory)
        {
            await _sqlExecutor.ExecuteAsync(async (connection, transaction) => {
                parentCategory.InstanceId = categoryId;
                await connection.InsertAsync(parentCategory, transaction);
            });
        }

        public Task<int> CreateCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateCategory(Core.Entities.Category category)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateCategoryAsync(Sparcpoint.Core.Entities.Category category)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) => {
                category.CreatedTimestamp = DateTime.Now;
                var instanceId = await connection.InsertAsync(category, transaction);
                if (category.CategoryAttributes.Count > 0)
                {
                    foreach (var item in category.CategoryAttributes)
                    {
                        item.InstanceId = instanceId;
                        await connection.InsertAsync(item, transaction);
                    }
                }
                if (category.Categories.Count > 0)
                {
                    foreach (var item in category.Categories)
                    {
                        item.InstanceId = instanceId;
                        await connection.InsertAsync(item, transaction);
                    }
                }
                return instanceId;

            });
        }

        public Task<List<Category>> GetCategories()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var orderBy = _sqlServerQueryProvider.OrderBy("Name", OrderByClause.OrderByDirection.Ascending).OrderByClause;
            string sql = "SELECT * FROM " + TableName + orderBy;
            var categories = await _queryService.QueryAsync<Category>(sql);
            return categories.ToList();
        }

        Task<List<Core.Entities.Category>> ICategoryService.GetCategories()
        {
            throw new NotImplementedException();
        }

        Task<List<Core.Entities.Category>> ICategoryService.GetCategoriesAsync()
        {
            throw new NotImplementedException();
        }
    }
}