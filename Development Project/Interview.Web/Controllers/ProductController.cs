using Interview.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    
    public class ProductController : Controller
    {

        // NOTE: Sample Action
        [Route("api/v1/products")]
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            using (SparcpointInventoryDatabaseContext db = new SparcpointInventoryDatabaseContext())
            {
                var prods = db.Products.Include(x => x.ProductAttributes).Include(x => x.ProductCategories).AsNoTracking().ToList();
                return Task.FromResult((IActionResult)Ok(prods));
            }
        }
        [Route("api/v1/products/AddProduct")]
        [HttpPost]
        public Task<IActionResult> AddProduct([FromBody][Bind("Name","Description","ProductImageUris", "ValidSkus", "ProductAttributes", "ProductCategories")]Product Product)
        {
            //get the Product from the body, JSON encoded, then add it to the Products and save
            using (SparcpointInventoryDatabaseContext db = new SparcpointInventoryDatabaseContext())
            {
                if (db.Products.Where(x => x.InstanceId == Product.InstanceId).Any())
                {
                    return Task.FromResult((IActionResult)BadRequest());
                }
                if(Product.ProductCategories.Count==0)
                {
                    return Task.FromResult((IActionResult)BadRequest());
                }
                Product.CreatedTimestamp = DateTime.Now;
                db.Products.Add(Product);
                db.SaveChanges();
                return Task.FromResult((IActionResult)Ok(Product));
            }
        }
        [Route("api/v1/products/SaveProduct")]
        [HttpGet]
        public Task<IActionResult> SaveProduct([FromBody][Bind("Name", "Description", "ProductImageUris", "ValidSkus", "ProductAttributes", "ProductCategories")] Product Product)
        {
            //get the Product from the body, JSON encoded, then mark it as modified and save
            using (SparcpointInventoryDatabaseContext db = new SparcpointInventoryDatabaseContext())
            {
                if (!db.Products.Where(x => x.InstanceId == Product.InstanceId).Any())
                {
                    return Task.FromResult((IActionResult)BadRequest());
                }
                if (Product.ProductCategories.Count == 0)
                {
                    return Task.FromResult((IActionResult)BadRequest());
                }
                Product.CreatedTimestamp = DateTime.Now;
                db.Entry(Product).State = EntityState.Modified;
                db.SaveChanges();
                return Task.FromResult((IActionResult)Ok(Product));
            }
        }
    }
}
