using Interview.Web.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        SparcpointInventoryDatabaseContext context;

        public ProductController(SparcpointInventoryDatabaseContext context)
        {
            this.context = context;
        }
        // EVAL: Endpoint to get a list of all the products
        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            var result = context.Products;

            return Ok(result);
        }

        // EVAL: Endpoint to get a list of products filtered by the category of the product or by their details
        [HttpGet]
        [Route("query")]
        public async Task<ActionResult> GetProductsByQuery([FromQuery] string category, [FromQuery] string detailsToSearch)
        {
            IQueryable<Product> result = null;
            if (category != null)
            {
                result = context.Products.Select(x => x.ProductCategories.Where(t => t.CategoryInstance.Name.Contains(category)).FirstOrDefault().Instance);
            }

            if (detailsToSearch != null)
            {
                result = context.Products.Where(x => x.Description.Contains(detailsToSearch));
            }


            return Ok(result);
        }


        // EVAL: Endpoint to get a count of the products in inventory
        [HttpGet]
        [Route("{productName}")]
        public async Task<ActionResult> GetProductCount([FromRoute] string productName)
        {
            var result = context.Products.Where(x => x.Name == productName);
            decimal productCount = context.InventoryTransactions.Where(x => x.ProductInstance == result).Select(x => x.Quantity).FirstOrDefault();

            return Ok(productCount);
        }

        // EVAL: Endpoint to create a product in the database 
        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] Product product)
        {
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            return Created("api/v1/products", product);
        }

        // EVAL: Endpoint to add inventory of a specified product 
        [HttpPost]
        public async Task<ActionResult> AddProductToInventory([FromBody] Product product, [FromQuery] int quantity)
        {
            InventoryTransaction invTransaction = new InventoryTransaction() { 
                ProductInstance = product,
                StartedTimestamp = System.DateTime.Today,
                Quantity = quantity                
            };
            await context.InventoryTransactions.AddAsync(invTransaction);
            await context.SaveChangesAsync();

            return Created("api/v1/products", product);
        }

        // EVAL: Endpoint to remove the inventory of a specified product
        [HttpDelete]
        public async Task<ActionResult> RemoveProductFromInventory([FromBody] Product product)
        {
            InventoryTransaction toRemove = context.InventoryTransactions.Where(x => x.ProductInstance == product).FirstOrDefault();
            context.InventoryTransactions.Remove(toRemove);
            await context.SaveChangesAsync();

            return Created("api/v1/products", product);
        }
    }
}
