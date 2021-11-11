using Interview.Web.Core.Repositories;
using Interview.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.DataServices.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(SparcpointContext dbContext)
        {
            ProductRepository = new ProductRepository(dbContext);
            CategoryRepository = new CategoryRepository(dbContext);
            ProductAttributesRepository = new ProductAttributesRepository(dbContext);
        }
        public ProductRepository ProductRepository {get; private set;}
        public CategoryRepository CategoryRepository { get; private set; }
        public ProductAttributesRepository ProductAttributesRepository { get; private set; }

        public int Complete()
        {
            return 0;
        }
        public void Dispose()
        {
            //throw new System.NotImplementedException();
        }
    }
}
