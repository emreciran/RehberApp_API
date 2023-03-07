using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Shared;

namespace RehberApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInfoFromLogined()
        {
            var response = await _userService.GetUserByUsername(HttpContext.User.Identity.Name);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(User user)
        {
            var response = await _userService.UpdateUserProfile(user);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(ChangePasswordViewModel model)
        {
            var response = await _userService.UpdateUserPassword(model);

            if(response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
