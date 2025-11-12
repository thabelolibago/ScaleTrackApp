using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IPasswordResetRepository;

namespace ScaleTrackAPI.Application.Features.Auth.Password.ResetPassword.BusinessRules.ResetPasswordBusinessRules
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
                return (false, AppError.Validation(ErrorMessages.Get("ResetPassword:Token")), null);

            if (string.IsNullOrWhiteSpace(newPassword))
                return (false, AppError.Validation(ErrorMessages.Get("ResetPassword:NewPasswordRequired")), null);

            if (newPassword != confirmPassword)
                return (false, AppError.Validation(ErrorMessages.Get("ResetPassword:PasswordsDoNotMatch")), null);

            if (newPassword.Length < 8)
                return (false, AppError.Validation(ErrorMessages.Get("ResetPassword:PasswordTooShort")), null);

            var tokenEntry = await _repo.GetTokenAsync(token);
            if (tokenEntry == null)
                return (false, AppError.NotFound(ErrorMessages.Get("ResetPassword:InvalidOrExpiredToken")), null);

            return (true, null, tokenEntry);
        }
    }
}


