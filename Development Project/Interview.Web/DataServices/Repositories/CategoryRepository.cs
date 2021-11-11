using Interview.Web.DataServices.Repositories;
using Interview.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Core.Repositories
{
    public class CategoryRepository : Repository<Categories>, ICategoryRepository
    {
        public CategoryRepository(SparcpointContext dbContext):base(dbContext)
        {

        }
    }
}
