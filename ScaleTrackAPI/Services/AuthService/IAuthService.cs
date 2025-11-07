using System.Security.Claims;
using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Services.Auth
{
    public interface IAuthService
    {
        Task<(LoginResponse? Entity, AppError? Error)> LoginAsync(LoginRequest request, ClaimsPrincipal actor);
        Task<AppError?> VerifyEmailAsync(string token);
        Task<AppError?> ResendVerificationEmailAsync(string email, string baseUrl);
        Task<(LoginResponse? Entity, AppError? Error)> RefreshTokenAsync(RefreshTokenRequest request, ClaimsPrincipal actor);
        Task<AppError?> LogoutAsync(LogoutRequest request, ClaimsPrincipal actor);
        Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user);
    }
}
