using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IRefreshTokenRepository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task SaveChangesAsync();
    }
}
