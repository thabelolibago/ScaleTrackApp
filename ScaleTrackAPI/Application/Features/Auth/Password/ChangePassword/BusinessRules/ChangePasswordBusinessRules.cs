using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IUserRepository;

namespace ScaleTrackAPI.Application.Features.Auth.Password.ChangePassword.BusinessRules.ChangePasswordBusinessRules
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
                return (false, AppError.Validation(ErrorMessages.Get("ChangePassword:CurrentPasswordRequired")));
            
            if (string.IsNullOrWhiteSpace(newPassword))
                return (false, AppError.Validation(ErrorMessages.Get("ChangePassword:NewPasswordRequired")));

            if (newPassword != confirmPassword)
                return (false, AppError.Validation(ErrorMessages.Get("ChangePassword:PasswordsDoNotMatch")));

            if (newPassword.Length < 8)
                return (false, AppError.Validation(ErrorMessages.Get("ChangePassword:PasswordTooShort")));

            var user = await _userRepo.GetById(userId);
            if (user == null)
                return (false, AppError.NotFound(ErrorMessages.Get("User:UserNotFound", userId)));

            // Optionally: check if new password is same as old
            return (true, null);
        }
    }
}
