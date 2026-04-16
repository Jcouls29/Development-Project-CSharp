using Interview.Web.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace Interview.Web.Repositories
{
    // Simple SQL-backed repository that uses ISqlExecutor for database operations.
    // Metadata and categories are stored as JSON in NVARCHAR(MAX) columns for simplicity.
    public class SqlProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _executor;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public SqlProductRepository(ISqlExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public Task<Product> AddAsync(Product product)
        {
            return _executor.ExecuteAsync(async (conn, trans) =>
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "INSERT INTO dbo.Products (Id, Name, CreatedAt, Metadata, Categories) VALUES (@Id, @Name, @CreatedAt, @Metadata, @Categories)";

                    var pId = cmd.CreateParameter(); pId.ParameterName = "@Id"; pId.Value = product.Id; cmd.Parameters.Add(pId);
                    var pName = cmd.CreateParameter(); pName.ParameterName = "@Name"; pName.Value = (object)product.Name ?? DBNull.Value; cmd.Parameters.Add(pName);
                    var pCreated = cmd.CreateParameter(); pCreated.ParameterName = "@CreatedAt"; pCreated.Value = product.CreatedAt; cmd.Parameters.Add(pCreated);

                    var metadataJson = product.Metadata == null ? null : JsonSerializer.Serialize(product.Metadata, _jsonOptions);
                    var pMetadata = cmd.CreateParameter(); pMetadata.ParameterName = "@Metadata"; pMetadata.Value = (object)metadataJson ?? DBNull.Value; cmd.Parameters.Add(pMetadata);

                    ((System.Data.Common.DbCommand)cmd).ExecuteNonQuery();
                }

                return product;
            });
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return _executor.ExecuteAsync(async (conn, trans) =>
            {
                var list = new List<Product>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "SELECT Id, Name, CreatedAt, Metadata, Categories FROM dbo.Products";
                    using (var reader = ((System.Data.Common.DbCommand)cmd).ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(MapReaderToProduct(reader));
                        }
                    }
                }
                return (IEnumerable<Product>)list;
            });
        }

        public Task<Product> GetByIdAsync(Guid id)
        {
            return _executor.ExecuteAsync(async (conn, trans) =>
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "SELECT Id, Name, CreatedAt, Metadata, Categories FROM dbo.Products WHERE Id = @Id";
                    var pId = cmd.CreateParameter(); pId.ParameterName = "@Id"; pId.Value = id; cmd.Parameters.Add(pId);

                    using (var reader = ((System.Data.Common.DbCommand)cmd).ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToProduct(reader);
                        }
                    }
                }

                return null;
            });
        }

        public Task<IEnumerable<Product>> SearchByMetadataAsync(Dictionary<string, string> metadataCriteria)
        {
            if (metadataCriteria == null || metadataCriteria.Count == 0)
                return GetAllAsync();

            // Simple approach: load all products and filter in-memory. Keeps SQL simple for demo purposes.
            return _executor.ExecuteAsync(async (conn, trans) =>
            {
                var all = new List<Product>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "SELECT Id, Name, CreatedAt, Metadata, Categories FROM dbo.Products";
                    using (var reader = ((System.Data.Common.DbCommand)cmd).ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            all.Add(MapReaderToProduct(reader));
                        }
                    }
                }

                var results = new List<Product>();
                foreach (var p in all)
                {
                    var match = true;
                    foreach (var kv in metadataCriteria)
                    {
                        if (!p.Metadata.TryGetValue(kv.Key, out var v)) { match = false; break; }
                        if (!string.Equals(v, kv.Value, StringComparison.OrdinalIgnoreCase)) { match = false; break; }
                    }
                    if (match) results.Add(p);
                }

                return (IEnumerable<Product>)results;
            });
        }

        private Product MapReaderToProduct(IDataReader reader)
        {
            var p = new Product();
            p.Id = reader.GetGuid(0);
            p.Name = reader.IsDBNull(1) ? null : reader.GetString(1);
            p.CreatedAt = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2);

            if (!reader.IsDBNull(3))
            {
                var meta = reader.GetString(3);
                try { p.Metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(meta, _jsonOptions); }
                catch { p.Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); }
            }
            else
            {
                p.Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            return p;
        }
    }
}
