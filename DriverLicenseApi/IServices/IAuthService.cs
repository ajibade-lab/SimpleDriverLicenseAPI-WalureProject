using DriverLicenseApi.DTOs.User;

namespace DriverLicenseApi.IServices
{
    public interface IAuthService
    {
        Task<(bool Succeeded, string Message, AuthResponseDTO? Data)> RegisterAsync(RegisterDTO model);
        Task<(bool Succeeded, string Message, AuthResponseDTO? Data)> LoginAsync(LoginDTO model);
    }
}
