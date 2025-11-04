using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Repositories
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

