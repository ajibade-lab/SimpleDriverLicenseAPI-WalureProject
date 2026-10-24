using DriverLicenseApi.DTOs.Applications;
using DriverLicenseApi.IServices;
using DriverLicenseApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DriverLicenseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Staff,Admin")]
    public class StaffController : ControllerBase
    {
        private readonly ILicenseApplicationService _service;

        public StaffController(ILicenseApplicationService service)
        {
            _service = service;
        }
                        
        
        [HttpGet("applications")]
        public async Task<IActionResult> GetAll([FromQuery] ApplicationStatus? status, [FromQuery] string? sortBy)
        {
            var apps = await _service.GetAllApplicationsAsync(status, sortBy);
            return Ok(apps);
        }


        [HttpPut("applications/{Guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDTO model)
        {
            var staffName = User.FindFirstValue(ClaimTypes.Name) ?? "Staff Member";
            var result = await _service.UpdateStatusAsync(id, model.Status, model.Comment, staffName);

            if (result == null) 
                return NotFound(new { Message = "Application not found." });

            return Ok(result);
        }


        [HttpPut("applications/{Guid}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectApplicationDTO model)
        {
            var staffName = User.FindFirstValue(ClaimTypes.Name) ?? "Staff Member";
            var result = await _service.RejectApplicationAsync(id, model.Reason, staffName);

            if (result == null) 
                return NotFound(new { Message = "Application not found." });

            return Ok(result);
        }


        [HttpPut("applications/{Guid}/collect")]
        public async Task<IActionResult> Collect(Guid id)
        {
            var staffName = User.FindFirstValue(ClaimTypes.Name) ?? "Staff Member";
            var result = await _service.MarkAsCollectedAsync(id, staffName);

            if (result == null) 
                return NotFound(new { Message = "Application not found." });

            return Ok(result);
        }


        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var summary = await _service.GetDashboardSummaryAsync();
            return Ok(summary);
        }
    }
}
 