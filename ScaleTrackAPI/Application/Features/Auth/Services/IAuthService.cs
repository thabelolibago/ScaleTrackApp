using System.Security.Claims;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Features.Auth.DTOs.Logout;
using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.Auth.Services
{
    public interface IAuthService
    {
        Task<AppError?> VerifyEmailAsync(string token);
        Task<AppError?> ResendVerificationEmailAsync(string email, string baseUrl);
        
        Task<AppError?> LogoutAsync(LogoutRequest request, ClaimsPrincipal actor);
        Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user);
    }
}
