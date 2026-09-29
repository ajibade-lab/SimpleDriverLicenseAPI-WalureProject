namespace DriverLicenseApi.Models
{
    public class ApplicationStatusHistory
    {
        public int Id { get; set; }
        public int LicenseApplicationId { get; set; }
        public LicenseApplication? LicenseApplication { get; set; }

        public ApplicationStatus Status { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime DateChanged { get; set; } = DateTime.UtcNow;
        public string ChangedBy { get; set; } = string.Empty;
    }
}
