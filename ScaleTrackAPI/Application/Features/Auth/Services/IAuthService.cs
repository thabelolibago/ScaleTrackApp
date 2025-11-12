using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.Auth.Services
{
    public interface IAuthService
    {
        
        Task<AppError?> ResendVerificationEmailAsync(string email, string baseUrl);
        
        
        Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user);
    }
}
