using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Services.Auth
{
    public interface IAuthService
    {
        Task<(LoginResponse? Entity, AppError? Error)> LoginAsync(LoginRequest request);
        Task<(LoginResponse? Entity, AppError? Error)> RefreshTokenAsync(RefreshTokenRequest request);
        Task<AppError?> LogoutAsync(LogoutRequest request);
    }
}
