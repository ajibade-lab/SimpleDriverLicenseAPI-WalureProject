using System.ComponentModel.DataAnnotations;

namespace DriverLicenseApi.DTOs.Applications
{
    public class RejectApplicationDTO
    {
        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
