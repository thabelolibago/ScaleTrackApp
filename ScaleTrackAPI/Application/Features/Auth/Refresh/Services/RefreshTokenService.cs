using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IRefreshTokenRepository;

namespace ScaleTrackAPI.Application.Features.Auth.Refresh.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IConfiguration _config;

        public RefreshTokenService(IConfiguration config, IRefreshTokenRepository refreshTokenRepo)
        {
            _config = config;
            _refreshTokenRepo = refreshTokenRepo;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _refreshTokenRepo.GetByTokenAsync(token);
        }

        public async Task MarkRefreshTokenUsedAsync(RefreshToken token)
        {
            token.IsUsed = true;
            await _refreshTokenRepo.UpdateAsync(token);
            await _refreshTokenRepo.SaveChangesAsync();
        }

        public async Task MarkRefreshTokenRevokedAsync(RefreshToken token)
        {
            token.IsRevoked = true;
            await _refreshTokenRepo.UpdateAsync(token);
            await _refreshTokenRepo.SaveChangesAsync();
        }
    }
}