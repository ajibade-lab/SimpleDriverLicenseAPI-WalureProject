using DriverLicenseApi.Models;
using System.ComponentModel.DataAnnotations;

namespace DriverLicenseApi.DTOs.Applications
{
    public class UpdateStatusDTO
    {
        [Required]
        public ApplicationStatus Status { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
