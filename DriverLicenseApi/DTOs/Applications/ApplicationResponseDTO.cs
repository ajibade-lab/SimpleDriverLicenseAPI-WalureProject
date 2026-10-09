namespace DriverLicenseApi.DTOs.Applications
{
    public class ApplicationResponseDTO
    {
        public Guid Id { get; set; }
        public string ApplicationNumber { get; set; } = string.Empty;
        public string ApplicantName { get; set; } = string.Empty;
        public string ApplicantEmail { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string NotificationMessage { get; set; } = string.Empty;
        public DateTime SubmittedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? CollectionDate { get; set; }
        public string? RejectionReason { get; set; }
        public List<ApplicationStatusHistoryDTO> StatusHistory { get; set; } = new();
    }
}
