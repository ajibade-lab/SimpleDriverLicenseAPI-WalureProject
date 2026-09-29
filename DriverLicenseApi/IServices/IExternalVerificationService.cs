namespace DriverLicenseApi.IServices
{
    public interface IExternalVerificationService
    {
        Task<bool> VerifyApplicantIdentityAsync(string email);
    }
}
