using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IPasswordResetRepository
{
    public interface IPasswordResetRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<PasswordResetToken?> GetTokenAsync(string token);
        Task AddResetTokenAsync(PasswordResetToken token);
        Task RemoveResetTokenAsync(PasswordResetToken token);
        Task UpdateUserPasswordAsync(User user);
    }
}

