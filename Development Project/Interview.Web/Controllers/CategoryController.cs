using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Core.DTOs;
using Sparcpoint.Core.Models;
using Sparcpoint.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = new List<CategoryDto>();

            foreach (var category in categories)
            {
                var parentIds = (await _categoryRepository.GetParentCategoryIdsAsync(category.InstanceId)).ToList();
                var childIds = (await _categoryRepository.GetChildCategoryIdsAsync(category.InstanceId)).ToList();

                categoryDtos.Add(new CategoryDto
                {
                    InstanceId = category.InstanceId,
                    Name = category.Name,
                    Description = category.Description,
                    CreatedTimestamp = category.CreatedTimestamp,
                    ParentCategoryIds = parentIds,
                    ChildCategoryIds = childIds
                });
            }

            return Ok(categoryDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return NotFound();

            var parentIds = (await _categoryRepository.GetParentCategoryIdsAsync(category.InstanceId)).ToList();
            var childIds = (await _categoryRepository.GetChildCategoryIdsAsync(category.InstanceId)).ToList();

            return Ok(new CategoryDto
            {
                InstanceId = category.InstanceId,
                Name = category.Name,
                Description = category.Description,
                CreatedTimestamp = category.CreatedTimestamp,
                ParentCategoryIds = parentIds,
                ChildCategoryIds = childIds
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var category = Category.Create(createDto.Name, createDto.Description);
            var categoryId = await _categoryRepository.AddAsync(category);

            if (createDto.ParentCategoryIds?.Any() == true)
            {
                await _categoryRepository.UpdateParentCategoryRelationshipsAsync(categoryId, createDto.ParentCategoryIds);
            }

            return CreatedAtAction(nameof(GetCategory), new { id = categoryId }, new { InstanceId = categoryId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null) return NotFound();

            var category = Category.Create(updateDto.Name, updateDto.Description);
            category.SetInstanceId(id);

            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.UpdateParentCategoryRelationshipsAsync(id, updateDto.ParentCategoryIds ?? new List<int>());

            return NoContent();
        }
    }
}
