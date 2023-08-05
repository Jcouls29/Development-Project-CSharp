using Interview.Web.Models;
using Interview.Web.Models.Intefaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
	[Route("api/v1/products")]
	[ApiController]
	public class ProductController : Controller
	{
		// currently working off of this db context object but would switch this out for the repository made in the Data project
		// Sparcpoint.Data.ProductRepository
		// That object would establish our connections and we'd call it to perform our actions in this controller
		private readonly SparcpointInventoryDatabaseContext _context;

		private static Product resultToProduct(Product product)
		{
			return new Product
			{
				InstanceId = product.InstanceId,
				Name = product.Name,
				Description = product.Description,
				ProductImageUris = product.ProductImageUris,
				ValidSkus = product.ValidSkus,
				// trx
				// attr
				// cat
			};
		}

		public ProductController(SparcpointInventoryDatabaseContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<IEntity>>> GetAllProducts()
		{
			return await _context.Products.Select(p => resultToProduct(p)).ToListAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<IEntity>> GetByInstanceId(int id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}
			return resultToProduct(product);
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Create([FromBody] Product product)
		{
			_context.Products.Add(product);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetByInstanceId), new { instanceId = product.InstanceId }, product);
		}
	}
}
