using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sparcpoint.Inventory.Model;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Interview.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly SparcpointInventoryDatabaseContext _dbContext;
        public ProductController(SparcpointInventoryDatabaseContext dbContext) //TODO:  Consider using the Dapper Equivalent
        {
            _dbContext = dbContext;
        }

        // GET: api/Product/all
        [HttpGet]
        public IEnumerable<Products> All()
        {
            return _dbContext.Products.ToList<Products>();
        }

        // GET api/Product/{id}
        [HttpGet("{id}")]
        public Products Get(int id)
        {
            return _dbContext.Products.Where(p => p.InstanceId == id).FirstOrDefault();
        }

        // POST api/Product
        [HttpPost]
        public void Post([FromBody] string value)
        {
            _dbContext.Products.Add(JsonConvert.DeserializeObject<Products>(value));
            _dbContext.SaveChanges();

            //TODO: Transaction Handling
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            _dbContext.Products.Update(JsonConvert.DeserializeObject<Products>(value));
            _dbContext.SaveChanges();

            //TODO: Transaction Handling
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            Products p = _dbContext.Products.Where(p => p.InstanceId == id).FirstOrDefault();
            if (p != default(Products)) {
                _dbContext.Products.Remove(p);
                _dbContext.SaveChanges();
            }

            //TODO: Transaction Handling
        }
    }
}
