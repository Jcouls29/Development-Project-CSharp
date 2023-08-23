using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Sparcpoint.Inventory.Application.Data;
using Sparcpoint.Inventory.Domain.Entities.Instances;

namespace Sparcpoint.Inventory.Application.Repositories
{
    public class SqlInstanceRepository<T>
    {
        private readonly FluentSqlExecutor _executor;
        private readonly string _tableName;

        public SqlInstanceRepository(ISqlExecutorFactory executor)
        {
            _executor = new FluentSqlExecutor(executor);
            _tableName = Pluralize(typeof(T).Name);
        }

        public virtual async Task<T> GetAsync(int id)
        {
            return await _executor
                .Select("*")
                .From(_tableName)
                .Where("InstanceId = @id")
                .WithParameters(new Dictionary<string, object> { { "id", id } })
                .ToSingleAsync<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _executor
                .Select("*")
                .From(_tableName)
                .ToListAsync<T>();
        }

        public virtual async Task<T> AddAsync(T item)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var values = properties.ToDictionary(prop => prop.Name, prop => prop.GetValue(item));

            await _executor
                .InsertInto(_tableName, values)
                .ExecuteNonQueryAsync();

            return item;
        }

        public virtual async Task<bool> RemoveAsync(int id)
        {
            int affectedRows = await _executor
                .DeleteFrom(_tableName)
                .Where("InstanceId = @id")
                .WithParameters(new Dictionary<string, object> { { "id", id } })
                .ExecuteNonQueryAsync();

            return affectedRows > 0;
        }

        protected static string Pluralize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;
            if (text.EndsWith("y") && !IsVowel(text[text.Length - 2]))
                return text.Substring(0, text.Length - 1) + "ies";
            if (text.EndsWith("s"))
                return text + "es";
            return text + "s";
        }
        private static bool IsVowel(char character)
        {
            character = char.ToLower(character);
            return character == 'a' || character == 'e' || character == 'i' || character == 'o' || character == 'u';
        }
    }

    public class SqlProductRepository :
        SqlInstanceRepository<Product>,
        IProductRepository
    {
        public SqlProductRepository(ISqlExecutorFactory factory)
            : base(factory)
        { }
    }

    public class SqlProductAttributeRepository :
        SqlInstanceRepository<ProductAttribute>,
        IProductAttributeRepository
    {
        public SqlProductAttributeRepository(ISqlExecutorFactory factory)
            : base(factory)
        { }
    }

    public class SqlProductCategoryRepository :
        SqlInstanceRepository<ProductCategory>,
        IProductCategoryRepository
    {
        public SqlProductCategoryRepository(ISqlExecutorFactory factory)
            : base(factory)
        { }
    }

    public class SqlCategoryRepository :
        SqlInstanceRepository<Category>,
        ICategoryRepository
    {
        public SqlCategoryRepository(ISqlExecutorFactory factory)
            : base(factory)
        { }
    }

    public class SqlCategoryAttributeRepository :
        SqlInstanceRepository<CategoryAttribute>,
        ICategoryAttributeRepository
    {
        public SqlCategoryAttributeRepository(ISqlExecutorFactory factory)
            : base(factory)
        { }
    }

    public class SqlCategoryCategoryRepository :
        SqlInstanceRepository<CategoryCategory>,
        ICategoryCategoryRepository
    {
        public SqlCategoryCategoryRepository(ISqlExecutorFactory factory)
            : base(factory)
        { }
    }
}
