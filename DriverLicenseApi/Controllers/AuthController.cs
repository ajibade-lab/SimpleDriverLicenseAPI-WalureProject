using DriverLicenseApi.DTOs.User;
using DriverLicenseApi.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DriverLicenseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.Succeeded) return BadRequest(new { Message = result.Message });
            return Ok(result.Data);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!result.Succeeded) return Unauthorized(new { Message = result.Message });
            return Ok(result.Data);
        }
    }
}
