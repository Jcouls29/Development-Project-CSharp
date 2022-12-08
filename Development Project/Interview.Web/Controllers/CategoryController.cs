using Interview.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    
    public class CategoryController : Controller
    {
        [Route("api/v1/Categories")]
        public Task<IActionResult> GetAllCategories()
        {
            using (SparcpointInventoryDatabaseContext db = new SparcpointInventoryDatabaseContext())
            {
                //asNoTracking just keeps it from adding a bunch of junk we don't need here.
                var prods = db.Categories.Include(x => x.CategoryAttributes).Include(x => x.CategoryCategoryInstances).AsNoTracking().ToList();
                return Task.FromResult((IActionResult)Ok(prods));
            }
        }
        [Route("api/v1/Categories/AddCategory")]
        [HttpPost]
        public Task<IActionResult> AddCategory([FromBody][Bind("Name", "Description", "CategoryAttributes", "CategoryCategoryInstances")] Category category)
        {
            //get the category from the body, JSON encoded, then add it to the categories and save
            using (SparcpointInventoryDatabaseContext db = new SparcpointInventoryDatabaseContext())
            {
                if (db.Categories.Where(x => x.InstanceId == category.InstanceId).Any())
                {
                    return Task.FromResult((IActionResult)BadRequest());
                }
                category.CreatedTimestamp = DateTime.Now;
                foreach(var _category in category.CategoryCategoryInstances)
                {
                    _category.CategoryInstance = category;
                }
                db.Categories.Add(category);

                db.SaveChanges();
                return Task.FromResult((IActionResult)Ok(category));
            }
        }
        [Route("api/v1/Categories/SaveCategory")]
        [HttpPost]
        public Task<IActionResult> SaveCategory([FromBody][Bind("InstanceId", "Name", "Description", "CategoryAttributes", "CategoryCategoryInstances")] Category category)
        {

            //get the category from the body, JSON encoded, then mark it as modified and save
            using (SparcpointInventoryDatabaseContext db = new SparcpointInventoryDatabaseContext())
            {
                if (!db.Categories.Where(x => x.InstanceId == category.InstanceId).Any())
                {
                    return Task.FromResult((IActionResult)BadRequest());
                }
                category.CreatedTimestamp = DateTime.Now;
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return Task.FromResult((IActionResult)Ok(category));
            }
        }

    }
}
