namespace DriverLicenseApi.DTOs.Applications
{
    public class ApplicationStatusHistoryDTO
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime DateChanged { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
    }
}
