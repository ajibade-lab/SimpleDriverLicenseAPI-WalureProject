namespace DriverLicenseApi.Models
{
    public class LicenseApplication
    {
        public int Id { get; set; }
        public string ApplicationNumber { get; set; } = string.Empty;

        // Foreign Key to ApplicationUser
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string LicenseType { get; set; } = "Standard"; // e.g., Standard, Heavy

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public DateTime? CollectionDate { get; set; }
        public string? RejectionReason { get; set; }

        // Navigation property for status history tracking
        public List<ApplicationStatusHistory> StatusHistories { get; set; } = new();
    }
}
