using System.ComponentModel.DataAnnotations;

namespace DriverLicenseApi.DTOs.User
{
    public class LoginDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(20, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }
}
