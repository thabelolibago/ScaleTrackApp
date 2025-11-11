using System.Security.Claims;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Features.Auth.DTOs.Login;
using ScaleTrackAPI.Application.Features.Auth.DTOs.Token;
using ScaleTrackAPI.Domain.Entities;


namespace ScaleTrackAPI.Application.Features.Auth.Services
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
