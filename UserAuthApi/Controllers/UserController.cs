using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserAuthApi.Services;

namespace UserAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // UserController handles user registration and login operations.
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        // Registers a new user with the provided credentials.
        public async Task<IActionResult> Register([FromBody] UserCredentials credentials)
        {
            var (success, message) = await _userService.RegisterAsync(credentials.Username, credentials.Password);
            if (!success) return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPost("login")]
        // Logs in a user with the provided credentials.
        public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
        {
            
            var (success, message) = await _userService.LoginAsync(credentials.Username, credentials.Password);
            if (!success) return Unauthorized(new { message });

            return Ok(new { message });
        }
    }

    // Represents user credentials for registration and login.
    public class UserCredentials
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}

