using DriverLicenseApi.DTOs.Applications;
using DriverLicenseApi.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DriverLicenseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationsController : ControllerBase
    {
        private readonly ILicenseApplicationService _service;

        public ApplicationsController(ILicenseApplicationService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Applicant")]
        public async Task<IActionResult> Create([FromBody] CreateApplicationDTO model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString)) 
                return Unauthorized();

            if (!Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            var result = await _service.CreateApplicationAsync(userId, model);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("my-application")]
        [Authorize(Roles = "Applicant")]
        public async Task<IActionResult> GetMyApplication()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString)) 
                return Unauthorized();

            if (!Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            var app = await _service.GetUserApplicationAsync(userId);

            if (app == null) 
                return NotFound(new { Message = "No application found for this account." });
            
            return Ok(app);
        }
                
        [HttpGet("{Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var app = await _service.GetByIdAsync(id);

            if (app == null) 
                return NotFound(new { Message = "Application not found." });

            return Ok(app);
        }

        [HttpGet("{Guid}/history")]
        public async Task<IActionResult> GetHistory(Guid id)
        {
            var history = await _service.GetHistoryAsync(id);
            return Ok(history);
        }
    }
}
