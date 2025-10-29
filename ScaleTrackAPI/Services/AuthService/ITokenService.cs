using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Services.Auth
{
    public interface ITokenService
    {
        Task<(string AccessToken, string RefreshToken)> CreateTokensAsync(User user);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task MarkRefreshTokenUsedAsync(RefreshToken token);
        Task MarkRefreshTokenRevokedAsync(RefreshToken token);
    }
}
