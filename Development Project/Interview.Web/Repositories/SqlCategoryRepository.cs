using Interview.Web.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Repositories
{
    public class SqlCategoryRepository : ICategoryRepository
    {
        private readonly ISqlExecutor _executor;

        public SqlCategoryRepository(ISqlExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public Task<Category> AddAsync(Category category)
        {
            return _executor.ExecuteAsync(async (conn, trans) =>
            {
                // Create table if missing
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = @"IF OBJECT_ID('dbo.Categories','U') IS NULL
                        BEGIN
                            CREATE TABLE dbo.Categories (
                                Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                                Name NVARCHAR(200) NULL,
                                DisplayName NVARCHAR(400) NULL,
                                ParentId UNIQUEIDENTIFIER NULL,
                                Department NVARCHAR(200) NULL,
                                SortOrder INT NULL,
                                IsActive BIT NOT NULL DEFAULT 1,
                                CreatedAt DATETIME2 NULL
                            )
                        END";
                    ((System.Data.Common.DbCommand)cmd).ExecuteNonQuery();
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "INSERT INTO dbo.Categories (Id, Name, DisplayName, ParentId, Department, SortOrder, IsActive, CreatedAt) VALUES (@Id, @Name, @DisplayName, @ParentId, @Department, @SortOrder, @IsActive, @CreatedAt)";
                    var idp = cmd.CreateParameter(); idp.ParameterName = "@Id"; idp.Value = category.Id; cmd.Parameters.Add(idp);
                    var n = cmd.CreateParameter(); n.ParameterName = "@Name"; n.Value = (object)category.Name ?? DBNull.Value; cmd.Parameters.Add(n);
                    var dn = cmd.CreateParameter(); dn.ParameterName = "@DisplayName"; dn.Value = (object)category.DisplayName ?? DBNull.Value; cmd.Parameters.Add(dn);
                    var pid = cmd.CreateParameter(); pid.ParameterName = "@ParentId"; pid.Value = (object)category.ParentId ?? DBNull.Value; cmd.Parameters.Add(pid);
                    var dep = cmd.CreateParameter(); dep.ParameterName = "@Department"; dep.Value = (object)category.Department ?? DBNull.Value; cmd.Parameters.Add(dep);
                    var so = cmd.CreateParameter(); so.ParameterName = "@SortOrder"; so.Value = category.SortOrder; cmd.Parameters.Add(so);
                    var ia = cmd.CreateParameter(); ia.ParameterName = "@IsActive"; ia.Value = category.IsActive; cmd.Parameters.Add(ia);
                    var ca = cmd.CreateParameter(); ca.ParameterName = "@CreatedAt"; ca.Value = category.CreatedAt; cmd.Parameters.Add(ca);

                    ((System.Data.Common.DbCommand)cmd).ExecuteNonQuery();
                }

                return category;
            });
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        {
            return _executor.ExecuteAsync(async (conn, trans) =>
            {
                var list = new List<Category>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "SELECT Id, Name, DisplayName, ParentId, Department, SortOrder, IsActive, CreatedAt FROM dbo.Categories";
                    using (var reader = ((System.Data.Common.DbCommand)cmd).ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var c = new Category();
                            c.Id = reader.GetGuid(0);
                            c.Name = reader.IsDBNull(1) ? null : reader.GetString(1);
                            c.DisplayName = reader.IsDBNull(2) ? null : reader.GetString(2);
                            c.ParentId = reader.IsDBNull(3) ? (Guid?)null : reader.GetGuid(3);
                            c.Department = reader.IsDBNull(4) ? null : reader.GetString(4);
                            c.SortOrder = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                            c.IsActive = !reader.IsDBNull(6) && reader.GetBoolean(6);
                            c.CreatedAt = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7);
                            list.Add(c);
                        }
                    }
                }
                return (IEnumerable<Category>)list;
            });
        }

        public Task<Category> GetByIdAsync(Guid id)
        {
            return _executor.ExecuteAsync(async (conn, trans) =>
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "SELECT Id, Name, DisplayName, ParentId, Department, SortOrder, IsActive, CreatedAt FROM dbo.Categories WHERE Id = @Id";
                    var idp = cmd.CreateParameter(); idp.ParameterName = "@Id"; idp.Value = id; cmd.Parameters.Add(idp);
                    using (var reader = ((System.Data.Common.DbCommand)cmd).ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var c = new Category();
                            c.Id = reader.GetGuid(0);
                            c.Name = reader.IsDBNull(1) ? null : reader.GetString(1);
                            c.DisplayName = reader.IsDBNull(2) ? null : reader.GetString(2);
                            c.ParentId = reader.IsDBNull(3) ? (Guid?)null : reader.GetGuid(3);
                            c.Department = reader.IsDBNull(4) ? null : reader.GetString(4);
                            c.SortOrder = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                            c.IsActive = !reader.IsDBNull(6) && reader.GetBoolean(6);
                            c.CreatedAt = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7);
                            return c;
                        }
                    }
                }
                return null;
            });
        }

        public Task<IEnumerable<Category>> GetChildrenAsync(Guid parentId)
        {
            return _executor.ExecuteAsync(async (conn, trans) =>
            {
                var list = new List<Category>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = "SELECT Id, Name, DisplayName, ParentId, Department, SortOrder, IsActive, CreatedAt FROM dbo.Categories WHERE ParentId = @ParentId";
                    var pid = cmd.CreateParameter(); pid.ParameterName = "@ParentId"; pid.Value = parentId; cmd.Parameters.Add(pid);
                    using (var reader = ((System.Data.Common.DbCommand)cmd).ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var c = new Category();
                            c.Id = reader.GetGuid(0);
                            c.Name = reader.IsDBNull(1) ? null : reader.GetString(1);
                            c.DisplayName = reader.IsDBNull(2) ? null : reader.GetString(2);
                            c.ParentId = reader.IsDBNull(3) ? (Guid?)null : reader.GetGuid(3);
                            c.Department = reader.IsDBNull(4) ? null : reader.GetString(4);
                            c.SortOrder = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                            c.IsActive = !reader.IsDBNull(6) && reader.GetBoolean(6);
                            c.CreatedAt = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7);
                            list.Add(c);
                        }
                    }
                }
                return (IEnumerable<Category>)list;
            });
        }

        public Task<IEnumerable<Category>> GetHierarchyAsync()
        {
            return GetAllAsync();
        }
    }
}
