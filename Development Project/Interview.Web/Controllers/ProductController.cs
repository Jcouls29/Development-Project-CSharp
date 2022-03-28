using Interview.Web.CustomModels;
using Interview.Web.Interfaces;
using Interview.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepo _productRepo;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductRepo productRepo, ILogger<ProductController> logger)
        {
            _productRepo = productRepo;
            _logger = logger;
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchProducts([FromBody] SearchInput input)
        {
            try
            {
                _logger.LogInformation("SearchProducts request started");
                //EVAL: Validating input request before processing the request
                if (ModelState.IsValid)
                {
                    var result = await _productRepo.SearchProducts(input);
                    return Ok(result);
                }
                else
                {
                    //EVAL: returning model state error message
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    _logger.LogError(message);
                    return StatusCode(StatusCodes.Status400BadRequest, message);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("SearchProducts request failed, see below exceptions");
                _logger.LogError(ex.Message.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("addorupdate")]
        public async Task<IActionResult> AddOrUpdateProduct([FromBody] Product input)
        {
            try
            {
                _logger.LogInformation("AddOrUpdateProduct request started");
                //EVAL: Validating input request before processing the request
                if (ModelState.IsValid)
                {
                    var result = await _productRepo.AddOrUpdateProduct(input);
                    return Ok(result);
                }
                else
                {
                    //EVAL: returning model state error message
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    _logger.LogError(message);
                    return StatusCode(StatusCodes.Status400BadRequest, message);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("AddOrUpdateProduct request failed, see below exceptions");
                _logger.LogError(ex.Message.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("remove")]
        public async Task<IActionResult> RemoveProduct(int instanceId)
        {
            try
            {
                _logger.LogInformation("RemoveProduct request started");
                var result = await _productRepo.RemoveProduct(instanceId);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError("RemoveProduct request failed, see below exceptions");
                _logger.LogError(ex.Message.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
