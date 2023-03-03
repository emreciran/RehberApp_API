using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace RehberApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private UserManager<IdentityUser> _userManager;
        private IUserRepository _userRepository;
        IConfiguration _configuration { get; set; }

        public AuthController(UserManager<IdentityUser> userManager, IUserRepository userRepository, IConfiguration configuration)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userRepository.RegisterUser(model);

                if(result.IsSuccess) return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userRepository.LoginUser(model);

                if (result.IsSuccess)
                {
                    var jwt = result.AuthResult.Token;

                    HttpContext.Response.Cookies.Append("jwt", jwt, new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTime.Now.AddDays(1),
                        SameSite = SameSiteMode.None,
                        IsEssential = true
                    });

                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("");
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userid, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(userid) || string.IsNullOrWhiteSpace(token))
            {
                return NotFound();
            }

            var result = await _userRepository.ConfirmEmail(userid, token);

            if (result.IsSuccess)
            {
                return Redirect(_configuration["ClientURL"] + "/auth/login?confirmEmail=" + token);
            }

            return BadRequest(result);
        }
    }
}
