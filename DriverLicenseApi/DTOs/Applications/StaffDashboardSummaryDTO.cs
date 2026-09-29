namespace DriverLicenseApi.DTOs.Applications
{
    public class StaffDashboardSummaryDTO
    {
        public int TotalApplications { get; set; }
        public Dictionary<string, int> StatusCounts { get; set; } = new();
    }
}
