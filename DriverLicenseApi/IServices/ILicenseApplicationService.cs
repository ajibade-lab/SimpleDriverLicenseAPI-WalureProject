using DriverLicenseApi.DTOs.Applications;
using DriverLicenseApi.Models;

namespace DriverLicenseApi.IServices
{
    public interface ILicenseApplicationService
    {
        Task<ApplicationResponseDTO> CreateApplicationAsync(Guid userId, CreateApplicationDTO dto);
        Task<ApplicationResponseDTO?> GetUserApplicationAsync(Guid userId);
        Task<ApplicationResponseDTO?> GetByIdAsync(Guid id);
        Task<List<ApplicationResponseDTO>> GetAllApplicationsAsync(ApplicationStatus? statusFilter, string? sortBy);
        Task<ApplicationResponseDTO?> UpdateStatusAsync(Guid id, ApplicationStatus newStatus, string comment, string changedBy);
        Task<ApplicationResponseDTO?> RejectApplicationAsync(Guid id, string reason, string changedBy);
        Task<ApplicationResponseDTO?> MarkAsCollectedAsync(Guid id, string changedBy);
        Task<List<ApplicationStatusHistoryDTO>> GetHistoryAsync(Guid id);
        Task<StaffDashboardSummaryDTO> GetDashboardSummaryAsync();
    }
}
