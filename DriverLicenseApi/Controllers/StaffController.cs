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

        [HttpPut("applications/{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDTO dto)
        {
            var staffName = User.FindFirstValue(ClaimTypes.Name) ?? "Staff Member";
            var result = await _service.UpdateStatusAsync(id, dto.Status, dto.Comment, staffName);
            if (result == null) return NotFound(new { Message = "Application not found." });
            return Ok(result);
        }

        [HttpPut("applications/{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectApplicationDTO dto)
        {
            var staffName = User.FindFirstValue(ClaimTypes.Name) ?? "Staff Member";
            var result = await _service.RejectApplicationAsync(id, dto.Reason, staffName);
            if (result == null) return NotFound(new { Message = "Application not found." });
            return Ok(result);
        }

        [HttpPut("applications/{id}/collect")]
        public async Task<IActionResult> Collect(int id)
        {
            var staffName = User.FindFirstValue(ClaimTypes.Name) ?? "Staff Member";
            var result = await _service.MarkAsCollectedAsync(id, staffName);
            if (result == null) return NotFound(new { Message = "Application not found." });
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
