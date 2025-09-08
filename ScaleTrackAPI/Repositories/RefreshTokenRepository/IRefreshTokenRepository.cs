namespace ScaleTrackAPI.Repositories
{
    using ScaleTrackAPI.Models;

    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task SaveChangesAsync();
    }
}
