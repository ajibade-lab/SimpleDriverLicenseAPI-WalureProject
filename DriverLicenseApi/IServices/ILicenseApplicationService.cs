using DriverLicenseApi.DTOs.Applications;
using DriverLicenseApi.Models;

namespace DriverLicenseApi.IServices
{
    public interface ILicenseApplicationService
    {
        Task<ApplicationResponseDto> CreateApplicationAsync(string userId, CreateApplicationDTO dto);
        Task<ApplicationResponseDto?> GetUserApplicationAsync(string userId);
        Task<ApplicationResponseDto?> GetByIdAsync(int id);
        Task<List<ApplicationResponseDto>> GetAllApplicationsAsync(ApplicationStatus? statusFilter, string? sortBy);
        Task<ApplicationResponseDto?> UpdateStatusAsync(int id, ApplicationStatus newStatus, string comment, string changedBy);
        Task<ApplicationResponseDto?> RejectApplicationAsync(int id, string reason, string changedBy);
        Task<ApplicationResponseDto?> MarkAsCollectedAsync(int id, string changedBy);
        Task<List<ApplicationStatusHistoryDto>> GetHistoryAsync(int id);
        Task<StaffDashboardSummaryDTO> GetDashboardSummaryAsync();
    }
}
