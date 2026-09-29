using System.ComponentModel.DataAnnotations;

namespace DriverLicenseApi.DTOs.Applications
{
    public class CreateApplicationDTO
    {
        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required, Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string LicenseType { get; set; } = "Standard";
    }
}
