using ScaleTrackAPI.Application.Errors.AppError;

namespace ScaleTrackAPI.Application.Features.Auth.ResendVerification.Services
{
    public interface IResendVerificationService
    {
        Task<AppError?> ResendVerificationEmailAsync(string email, string baseUrl);
    }
}