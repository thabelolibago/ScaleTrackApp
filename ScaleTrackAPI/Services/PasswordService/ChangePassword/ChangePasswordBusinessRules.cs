using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Repositories;

namespace ScaleTrackAPI.Services
{
    public class ChangePasswordBusinessRules
    {
        private readonly IUserRepository _userRepo;

        public ChangePasswordBusinessRules(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<(bool IsValid, AppError? Error)> ValidateChangePasswordAsync(
            int userId, string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword))
                return (false, AppError.Validation("Current password is required."));
            
            if (string.IsNullOrWhiteSpace(newPassword))
                return (false, AppError.Validation("New password is required."));

            if (newPassword != confirmPassword)
                return (false, AppError.Validation("Passwords do not match."));

            if (newPassword.Length < 8)
                return (false, AppError.Validation("Password must be at least 8 characters long."));

            var user = await _userRepo.GetById(userId);
            if (user == null)
                return (false, AppError.NotFound("User not found."));

            // Optionally: check if new password is same as old
            return (true, null);
        }
    }
}
