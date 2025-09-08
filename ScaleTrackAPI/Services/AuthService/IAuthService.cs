using ScaleTrackAPI.DTOs.Auth;

namespace ScaleTrackAPI.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request);
        Task<bool> LogoutAsync(LogoutRequest request);
    }
}
