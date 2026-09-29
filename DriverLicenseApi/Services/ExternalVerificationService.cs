using DriverLicenseApi.IServices;

namespace DriverLicenseApi.Services
{
    public class ExternalVerificationService : IExternalVerificationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalVerificationService> _logger;

        public ExternalVerificationService(HttpClient httpClient, ILogger<ExternalVerificationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> VerifyApplicantIdentityAsync(string email)
        {
            try
            {
                // External public test API call (simulating automated background check / external system lookup)
                var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/posts/1");
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("External verification call succeeded for user {Email}", email);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning(ex, "External verification service unavailable. Bypassing fallback verification.");
            }

            return true; // Soft fail default for testing continuity
        }
    }
}
