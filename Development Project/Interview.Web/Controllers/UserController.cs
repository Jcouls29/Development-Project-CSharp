using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint;
using Sparcpoint.BusinessLayer.Product;
using Sparcpoint.BusinessLayer.User;
using Sparcpoint.Mappers.DomainToEntity;
using Sparcpoint.Models;
using Sparcpoint.Models.DomainDto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/User")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
 
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            var token = _userService.Login(user.UserName, user.Password);

            if (token == null || token == String.Empty)
                return BadRequest(new { message = "User name or password is incorrect" });

            return Ok(token);
        }

    }
}
