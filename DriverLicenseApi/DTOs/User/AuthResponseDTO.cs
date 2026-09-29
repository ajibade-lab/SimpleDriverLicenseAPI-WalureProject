namespace DriverLicenseApi.DTOs.User
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
