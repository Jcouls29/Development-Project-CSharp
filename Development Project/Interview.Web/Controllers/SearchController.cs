using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sparcpoint.Inventory.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Interview.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly SparcpointInventoryDatabaseContext _dbContext;
        public SearchController(SparcpointInventoryDatabaseContext dbContext) //TODO:  Consider using the Dapper Equivalent
        {
            _dbContext = dbContext;
        }


        //  api/Search/ByCategory/{value}
        [HttpGet("{value}")]
        public IEnumerable<Products> ByCategory(string category)  // TODO: Consider making this a multi-category search.  Also, we're assuming that we just receive the category name.
        {
            Categories cat = _dbContext.Categories.Where(cat => cat.Name.Equals(category)).FirstOrDefault();
            if (cat != default(Categories)) {

                List<ProductCategories> pc = _dbContext.ProductCategories.Where(pc => pc.CategoryInstanceId == cat.InstanceId).ToList();
                return _dbContext.Products.Where(p => pc.Exists(item => item.InstanceId == p.InstanceId));
            }
            else
            {
                return new List<Products>();
            }

        }
        
        //TODO:  Implement a search by metadata as well as product details
    }
}
