using DriverLicenseApi.Data;
using DriverLicenseApi.DTOs.Applications;
using DriverLicenseApi.IServices;
using DriverLicenseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DriverLicenseApi.Services
{
    public class LicenseApplicationService : ILicenseApplicationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExternalVerificationService _verificationService;
        private readonly ILogger<LicenseApplicationService> _logger;

        public LicenseApplicationService(
            ApplicationDbContext context,
            IExternalVerificationService verificationService,
            ILogger<LicenseApplicationService> logger)
        {
            _context = context;
            _verificationService = verificationService;
            _logger = logger;
        }

        public async Task<ApplicationResponseDto> CreateApplicationAsync(string userId, CreateApplicationDTO dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new InvalidOperationException("User not found.");

            // Optional background check via external HttpClient service
            await _verificationService.VerifyApplicantIdentityAsync(user.Email!);

            // Generate application reference number e.g., DL-2026-XXXXXX
            var referenceNumber = $"DL-{DateTime.UtcNow.Year}-{Random.Shared.Next(100000, 999999)}";

            var application = new LicenseApplication
            {
                UserId = userId,
                ApplicationNumber = referenceNumber,
                DateOfBirth = dto.DateOfBirth,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                LicenseType = dto.LicenseType,
                Status = ApplicationStatus.Submitted,
                SubmittedDate = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            application.StatusHistories.Add(new ApplicationStatusHistory
            {
                Status = ApplicationStatus.Submitted,
                Comment = "Application submitted successfully.",
                DateChanged = DateTime.UtcNow,
                ChangedBy = user.FullName
            });

            _context.LicenseApplications.Add(application);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New application submitted: {AppNum} by User {UserId}", referenceNumber, userId);

            return MapToDto(application, user.FullName, user.Email!);
        }

        public async Task<ApplicationResponseDto?> GetUserApplicationAsync(string userId)
        {
            var app = await _context.LicenseApplications
                .Include(a => a.User)
                .Include(a => a.StatusHistories)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.SubmittedDate)
                .FirstOrDefaultAsync();

            return app == null ? null : MapToDto(app, app.User?.FullName ?? "", app.User?.Email ?? "");
        }

        public async Task<ApplicationResponseDto?> GetByIdAsync(int id)
        {
            var app = await _context.LicenseApplications
                .Include(a => a.User)
                .Include(a => a.StatusHistories)
                .FirstOrDefaultAsync(a => a.Id == id);

            return app == null ? null : MapToDto(app, app.User?.FullName ?? "", app.User?.Email ?? "");
        }

        public async Task<List<ApplicationResponseDto>> GetAllApplicationsAsync(ApplicationStatus? statusFilter, string? sortBy)
        {
            var query = _context.LicenseApplications
                .Include(a => a.User)
                .Include(a => a.StatusHistories)
                .AsQueryable();

            // LINQ Filtering
            if (statusFilter.HasValue)
            {
                query = query.Where(a => a.Status == statusFilter.Value);
            }

            // LINQ Sorting
            query = sortBy?.ToLower() switch
            {
                "date_asc" => query.OrderBy(a => a.SubmittedDate),
                "status" => query.OrderBy(a => a.Status),
                _ => query.OrderByDescending(a => a.SubmittedDate)
            };

            var applications = await query.ToListAsync();

            // LINQ Projection
            return applications.Select(a => MapToDto(a, a.User?.FullName ?? "", a.User?.Email ?? "")).ToList();
        }

        public async Task<ApplicationResponseDto?> UpdateStatusAsync(int id, ApplicationStatus newStatus, string comment, string changedBy)
        {
            var app = await _context.LicenseApplications
                .Include(a => a.User)
                .Include(a => a.StatusHistories)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (app == null) return null;

            app.Status = newStatus;
            app.LastUpdated = DateTime.UtcNow;

            app.StatusHistories.Add(new ApplicationStatusHistory
            {
                Status = newStatus,
                Comment = string.IsNullOrWhiteSpace(comment) ? $"Status changed to {newStatus}" : comment,
                DateChanged = DateTime.UtcNow,
                ChangedBy = changedBy
            });

            await _context.SaveChangesAsync();
            return MapToDto(app, app.User?.FullName ?? "", app.User?.Email ?? "");
        }

        public async Task<ApplicationResponseDto?> RejectApplicationAsync(int id, string reason, string changedBy)
        {
            var app = await _context.LicenseApplications
                .Include(a => a.User)
                .Include(a => a.StatusHistories)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (app == null) return null;

            app.Status = ApplicationStatus.Rejected;
            app.RejectionReason = reason;
            app.LastUpdated = DateTime.UtcNow;

            app.StatusHistories.Add(new ApplicationStatusHistory
            {
                Status = ApplicationStatus.Rejected,
                Comment = $"Application rejected. Reason: {reason}",
                DateChanged = DateTime.UtcNow,
                ChangedBy = changedBy
            });

            await _context.SaveChangesAsync();
            return MapToDto(app, app.User?.FullName ?? "", app.User?.Email ?? "");
        }

        public async Task<ApplicationResponseDto?> MarkAsCollectedAsync(int id, string changedBy)
        {
            var app = await _context.LicenseApplications
                .Include(a => a.User)
                .Include(a => a.StatusHistories)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (app == null) return null;

            app.Status = ApplicationStatus.Collected;
            app.CollectionDate = DateTime.UtcNow;
            app.LastUpdated = DateTime.UtcNow;

            app.StatusHistories.Add(new ApplicationStatusHistory
            {
                Status = ApplicationStatus.Collected,
                Comment = "Driver's license collected by applicant.",
                DateChanged = DateTime.UtcNow,
                ChangedBy = changedBy
            });

            await _context.SaveChangesAsync();
            return MapToDto(app, app.User?.FullName ?? "", app.User?.Email ?? "");
        }

        public async Task<List<ApplicationStatusHistoryDto>> GetHistoryAsync(int id)
        {
            return await _context.ApplicationStatusHistories
                .Where(h => h.LicenseApplicationId == id)
                .OrderBy(h => h.DateChanged)
                .Select(h => new ApplicationStatusHistoryDto
                {
                    Id = h.Id,
                    Status = h.Status.ToString(),
                    Comment = h.Comment,
                    DateChanged = h.DateChanged,
                    ChangedBy = h.ChangedBy
                })
                .ToListAsync();
        }

        // LINQ Simple Aggregation (Staff Summary)
        public async Task<StaffDashboardSummaryDTO> GetDashboardSummaryAsync()
        {
            var total = await _context.LicenseApplications.CountAsync();
            var grouped = await _context.LicenseApplications
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);

            return new StaffDashboardSummaryDTO
            {
                TotalApplications = total,
                StatusCounts = grouped
            };
        }

        private static ApplicationResponseDto MapToDto(LicenseApplication app, string fullName, string email)
        {
            string message = app.Status switch
            {
                ApplicationStatus.Submitted => "Your application has been received.",
                ApplicationStatus.UnderReview => "Your application is currently under review by staff.",
                ApplicationStatus.Approved => "Your application has been approved!",
                ApplicationStatus.Processing => "Your license card is currently being processed.",
                ApplicationStatus.ReadyForCollection => "Your driver's license is ready for collection.",
                ApplicationStatus.Collected => "License collected.",
                ApplicationStatus.Rejected => $"Application rejected. Reason: {app.RejectionReason}",
                _ => "Status updated."
            };

            return new ApplicationResponseDto
            {
                Id = app.Id,
                ApplicationNumber = app.ApplicationNumber,
                ApplicantName = fullName,
                ApplicantEmail = email,
                DateOfBirth = app.DateOfBirth,
                Address = app.Address,
                PhoneNumber = app.PhoneNumber,
                LicenseType = app.LicenseType,
                Status = app.Status.ToString(),
                NotificationMessage = message,
                SubmittedDate = app.SubmittedDate,
                LastUpdated = app.LastUpdated,
                CollectionDate = app.CollectionDate,
                RejectionReason = app.RejectionReason,
                StatusHistory = app.StatusHistories.Select(h => new ApplicationStatusHistoryDto
                {
                    Id = h.Id,
                    Status = h.Status.ToString(),
                    Comment = h.Comment,
                    DateChanged = h.DateChanged,
                    ChangedBy = h.ChangedBy
                }).OrderBy(h => h.DateChanged).ToList()
            };
        }
    }
}
