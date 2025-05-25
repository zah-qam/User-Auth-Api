using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserAuthApi.Services;

namespace UserAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCredentials credentials)
        {
            var (success, message) = await _userService.RegisterAsync(credentials.Username, credentials.Password);
            if (!success) return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
        {
            var (success, message) = await _userService.LoginAsync(credentials.Username, credentials.Password);
            if (!success) return Unauthorized(new { message });

            return Ok(new { message });
        }
    }

    public class UserCredentials
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
}
