using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.Auth.Refresh.Services
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task MarkRefreshTokenUsedAsync(RefreshToken token);
        Task MarkRefreshTokenRevokedAsync(RefreshToken token);
    }
}