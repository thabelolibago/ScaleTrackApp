using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Messages.SuccessMessages;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IUserRepository;
using ScaleTrackAPI.Shared.Helpers;

namespace ScaleTrackAPI.Application.Features.Auth.BusinessRules.AuthBusinessRules
{
    public class AuthBusinessRules
    {
        private readonly IUserRepository _userRepo;
        private readonly EmailHelper _emailHelper;

        public AuthBusinessRules(IUserRepository userRepo, EmailHelper emailHelper)
        {
            _userRepo = userRepo;
            _emailHelper = emailHelper;
        }

        public async Task<(AppError? Error, string? Token)> GenerateEmailVerificationAsync(User user, string baseUrl)
        {
            if (user.IsEmailVerified)
                return (AppError.Conflict(ErrorMessages.Get("EmailAlreadyExists", user.Email!)), null);

            user.EmailVerificationToken = Guid.NewGuid().ToString("N");
            user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);

            await _userRepo.Update(user);

            string verifyLink = $"{baseUrl}/verify-email?token={user.EmailVerificationToken}";
            string body = _emailHelper.BuildEmailVerificationEmail(user, verifyLink);

            await _emailHelper.SendEmailAsync(user.Email!, "Verify Your Email", body);

            return (null, user.EmailVerificationToken);
        }

        
    }
}

