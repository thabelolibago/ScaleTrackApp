using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Models;
using ScaleTrackAPI.Repositories;

namespace ScaleTrackAPI.Services.ResetPassword
{
    public class ResetPasswordBusinessRules
    {
        private readonly IPasswordResetRepository _repo;

        public ResetPasswordBusinessRules(IPasswordResetRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Validates a password reset token and new password.
        /// </summary>
        public async Task<(bool IsValid, AppError? Error, PasswordResetToken? Token)> ValidateResetAsync(
            string token, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(token))
                return (false, AppError.Validation("Token is required."), null);

            if (string.IsNullOrWhiteSpace(newPassword))
                return (false, AppError.Validation("New password is required."), null);

            if (newPassword != confirmPassword)
                return (false, AppError.Validation("Passwords do not match."), null);

            if (newPassword.Length < 8)
                return (false, AppError.Validation("Password must be at least 8 characters long."), null);

            var tokenEntry = await _repo.GetTokenAsync(token);
            if (tokenEntry == null)
                return (false, AppError.NotFound("Invalid or expired token."), null);

            return (true, null, tokenEntry);
        }
    }
}


