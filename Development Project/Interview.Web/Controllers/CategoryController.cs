using Interview.Web.Models;
using Interview.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/categories")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        // EVAL: The ability to add categories to the System with a parent-child hierarchy. Categories can be nested to any depth.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category c)
        {
            if (c == null) return BadRequest();
            var created = await _service.AddAsync(c);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _service.GetAllAsync();
            return Ok(all);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var c = await _service.GetByIdAsync(id);
            if (c == null) return NotFound();
            return Ok(c);
        }

        // EVAL: The ability to categorize and create hierarchies of products
        [HttpGet("hierarchy")]
        public async Task<IActionResult> GetHierarchy()
        {
            var all = (await _service.GetHierarchyAsync()).ToList();
            var lookup = all.ToLookup(x => x.ParentId);
            List<object> Build(Guid? parentId)
            {
                var list = new List<object>();
                foreach (var item in lookup[parentId])
                {
                    list.Add(new {
                        id = item.Id,
                        name = item.Name,
                        displayName = item.DisplayName,
                        children = Build(item.Id)
                    });
                }
                return list;
            }

            var tree = Build(null);
            return Ok(tree);
        }
    }
}
