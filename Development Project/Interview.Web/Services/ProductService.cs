using Interview.Web.Interfaces;
using Interview.Web.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Interview.Web.CustomModels;

namespace Interview.Web.Services
{
    public class ProductService : IProductRepo
    {
        private readonly SqlDbContext _dbContext;
        public ProductService(SqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //EVAL: Try catch blocks are added on the controller and these methods exceptions will be caught there.
        //EVAL: If there are any generic methods that are used else or have to continue flow even in exception can have their own try catch blocks 
        public async Task<object> SearchProducts(SearchInput input)
        {
            //EVAL: Joining tables that are needed for product search
            return await _dbContext.Products
            .Join(
                _dbContext.Categories,
                prdct => prdct.InstanceId,
                cat => cat.InstanceId,
                (prdct, cat) => new { Prodct = prdct, Catgry = cat })
            .Join(
                _dbContext.ProductAttributes,
                ps => ps.Prodct.InstanceId,
                pa => pa.InstanceId,
                (ps, pa) => new { ProdctCat = ps, ProdctAtt = pa })
            .Where(x => x.ProdctCat.Prodct.Name.Equals(input.ProductName)
            || x.ProdctCat.Prodct.Description.Equals(input.ProductDescription)
            || x.ProdctAtt.Key.Equals(input.ProductKey)
            || x.ProdctCat.Catgry.Equals(input.ProductCategory))
            .Select(c => new
            {
                c.ProdctCat.Prodct.ProductImageUris,
                c.ProdctCat.Prodct.Name,
                c.ProdctCat.Prodct.Description,
                c.ProdctCat.Prodct.CreatedTimestamp,
                c.ProdctCat.Prodct.ValidSkus,
            }).ToListAsync();
        }
        public async Task<bool> AddOrUpdateProduct(Product product)
        {
            //EVAL: If InstanceId is greater than 0 update or add new Product record
            if (product.InstanceId > 0)
            {
                _dbContext.Products.Update(product);
            }
            else
            {
                _dbContext.Add(product);
            }
            return await SaveAsync();
        }
        public async Task<bool> RemoveProduct(int instanceId)
        {
            var product = await _dbContext.Products.Where(x => x.InstanceId == instanceId).FirstOrDefaultAsync();
            _dbContext.Products.Remove(product);
            return await SaveAsync();
        }
        public async Task<bool> SaveAsync()
        {
            return (await _dbContext.SaveChangesAsync() > 0);
        }
    }
}
