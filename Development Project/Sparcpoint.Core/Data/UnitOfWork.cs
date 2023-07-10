using Dapper;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool disposedValue;
        private readonly IDbConnection connection;

        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }

        public UnitOfWork(IDbConnection connection)
        {
            this.connection = connection;
            Products = new ProductRepository(connection);
            Categories = new CategoryRepository(connection);
        }

        public async Task<int> SaveChangesAsync()
        {
            int affectedRows = 0;
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var products = Products.GetAllAsync(transaction); //Gets all products from repository
                    var categories = Categories.GetAllAsync(transaction);

                    foreach (var product in await products)
                    {
                        if (product.IsNew)
                        {
                            await Products.AddAsync(product, transaction);
                        }
                        else if (product.IsModified)
                        {
                            await Products.UpdateAsync(product, transaction);
                        }
                        else if (product.IsDeleted)
                        {
                            await Products.DeleteAsync(product.Id, transaction);
                        }
                        affectedRows++;
                    }

                    foreach (var category in await categories)
                    {
                        if (category.IsNew)
                        {
                            await Categories.AddAsync(category, transaction);
                        }
                        else if (category.IsModified)
                        {
                            await Categories.UpdateAsync(category, transaction);
                        }
                        else if (category.IsDeleted)
                        {
                            await Categories.DeleteAsync(category.Id, transaction);
                        }
                        affectedRows++;
                    }

                    transaction.Commit();
                    return affectedRows;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~UnitOfWork()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
