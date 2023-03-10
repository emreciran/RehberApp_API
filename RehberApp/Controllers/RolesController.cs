using BusinessLayer.Abstract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace RehberApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RolesController : ControllerBase
    {
        private IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRoles();
            if(roles == null) return NotFound();

            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var result = await _roleService.CreateRole(roleName);

            if (result.IsSuccess) return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            var result = await _roleService.AddUserToRole(email, roleName);

            if (result.IsSuccess) return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var roles = await _roleService.GetUserRoles(email);
            if (roles == null) return NotFound();
            return Ok(roles);
        }

        [HttpPost("RemoveUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole(string email, string roleName)
        {
            var result = await _roleService.RemoveUserFromRole(email, roleName);

            if (result.IsSuccess) return Ok(result);

            return BadRequest(result);
        }
    }
}
