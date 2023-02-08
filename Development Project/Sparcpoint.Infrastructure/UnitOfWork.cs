using Sparcpoint.Infrastructure.Repositories;
using Sparcpoint.Interfaces;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Threading.Tasks;

namespace Sparcpoint.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISqlExecutor sqlExecutor;

        public UnitOfWork(ISqlExecutor sqlExecutor)
        {
            this.sqlExecutor = sqlExecutor;
        }
        public IProductsRepository ProductsRepository => new ProductsRepository(sqlExecutor);
    }
}
