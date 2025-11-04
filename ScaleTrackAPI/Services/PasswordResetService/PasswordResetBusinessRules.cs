using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Models;
using ScaleTrackAPI.Repositories;

namespace ScaleTrackAPI.Services
{
    public class PasswordResetBusinessRules
    {
        private readonly IPasswordResetRepository _repo;

        public PasswordResetBusinessRules(IPasswordResetRepository repo)
        {
            _repo = repo;
        }

        public async Task<(bool IsValid, AppError? Error, User? User)> ValidateForgotPasswordAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, AppError.Validation("Email is required."), null);

            var user = await _repo.GetUserByEmailAsync(email);
            if (user == null)
                return (false, AppError.NotFound("User not found."), null);

            return (true, null, user);
        }

        public async Task<(bool IsValid, AppError? Error, PasswordResetToken? Token)> ValidateResetPasswordAsync(
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

