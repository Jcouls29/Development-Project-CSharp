// <auto-generated>
// ReSharper disable All

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sparkpoint.Data
{
    // ****************************************************************************************************
    // This is not a commercial licence, therefore only a few tables/views/stored procedures are generated.
    // ****************************************************************************************************

    public class FakeInventoryContext : IInventoryContext
    {
        public DbSet<Category> Categories { get; set; } // Categories
        public DbSet<CategoryAttribute> CategoryAttributes { get; set; } // CategoryAttributes
        public DbSet<CategoryCategory> CategoryCategories { get; set; } // CategoryCategories
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; } // InventoryTransactions
        public DbSet<Product> Products { get; set; } // Products
        public DbSet<ProductAttribute> ProductAttributes { get; set; } // ProductAttributes
        public DbSet<ProductCategory> ProductCategories { get; set; } // ProductCategories

        public FakeInventoryContext()
        {
            _database = new FakeDatabaseFacade(new InventoryContext());

            Categories = new FakeDbSet<Category>("InstanceId");
            CategoryAttributes = new FakeDbSet<CategoryAttribute>("InstanceId", "Key");
            CategoryCategories = new FakeDbSet<CategoryCategory>("InstanceId", "CategoryInstanceId");
            InventoryTransactions = new FakeDbSet<InventoryTransaction>("TransactionId");
            Products = new FakeDbSet<Product>("InstanceId");
            ProductAttributes = new FakeDbSet<ProductAttribute>("InstanceId", "Key");
            ProductCategories = new FakeDbSet<ProductCategory>("InstanceId", "CategoryInstanceId");

        }

        public int SaveChangesCount { get; private set; }
        public virtual int SaveChanges()
        {
            ++SaveChangesCount;
            return 1;
        }

        public virtual int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return SaveChanges();
        }

        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            ++SaveChangesCount;
            return Task<int>.Factory.StartNew(() => 1, cancellationToken);
        }
        public virtual Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
        {
            ++SaveChangesCount;
            return Task<int>.Factory.StartNew(x => 1, acceptAllChangesOnSuccess, cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private DatabaseFacade _database;
        public DatabaseFacade Database { get { return _database; } }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry Add(object entity)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual Task AddRangeAsync(params object[] entities)
        {
            throw new NotImplementedException();
        }

        public virtual async Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual void AddRange(IEnumerable<object> entities)
        {
            throw new NotImplementedException();
        }

        public virtual void AddRange(params object[] entities)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry Attach(object entity)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual void AttachRange(IEnumerable<object> entities)
        {
            throw new NotImplementedException();
        }

        public virtual void AttachRange(params object[] entities)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry Entry(object entity)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual TEntity Find<TEntity>(params object[] keyValues) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual ValueTask<TEntity> FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual ValueTask<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual ValueTask<object> FindAsync(Type entityType, object[] keyValues, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual ValueTask<object> FindAsync(Type entityType, params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public virtual object Find(Type entityType, params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry Remove(object entity)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveRange(IEnumerable<object> entities)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveRange(params object[] entities)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry Update(object entity)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateRange(IEnumerable<object> entities)
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateRange(params object[] entities)
        {
            throw new NotImplementedException();
        }

    }
}
// </auto-generated>
