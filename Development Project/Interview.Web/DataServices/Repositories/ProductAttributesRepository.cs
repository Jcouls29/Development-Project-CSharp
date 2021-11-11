using Interview.Web.DataServices.Repositories;
using Interview.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Core.Repositories
{
    public class ProductAttributesRepository : Repository<ProductAttributes>, IProductAttributesRepository
    {
        public ProductAttributesRepository(SparcpointContext dbContext):base(dbContext)
        {

        }
    }
}
