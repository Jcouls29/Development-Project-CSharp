using AutoMapper;
using Interview.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Abstract.Services;
using Sparcpoint.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventory")]
    public class InventoryController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService, IMapper mapper)
        {
            _mapper = mapper;
            _inventoryService = inventoryService;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInventory([FromBody] List<UpdateInventoryDto> requestDto)
        {
            if (requestDto != null && requestDto.Any())
            {
                try
                {
                    var request = _mapper.Map<List<UpdateInventoryRequestDto>>(requestDto);
                    await _inventoryService.UpdateInventoryAsync(request);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            return BadRequest("Invalid inventory data.");

        }

    }
}
