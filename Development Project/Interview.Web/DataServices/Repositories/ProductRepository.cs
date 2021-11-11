using Interview.Web.DataServices.Repositories;
using Interview.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Core.Repositories
{
    public class ProductRepository: Repository<Products>, IProductRepository
    {
        public ProductRepository(SparcpointContext dbContext):base(dbContext)
        {

        }
    }
}
