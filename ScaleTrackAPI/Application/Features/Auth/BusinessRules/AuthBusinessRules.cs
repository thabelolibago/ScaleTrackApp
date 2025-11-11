using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Auth.DTOs.Login;
using ScaleTrackAPI.Application.Features.Auth.DTOs.Token;
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

        public async Task<AppError?> ValidateLoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return AppError.Validation(ErrorMessages.Get("Validation:EmailRequired"));

            if (string.IsNullOrWhiteSpace(request.Password))
                return AppError.Validation(ErrorMessages.Get("Validation:PasswordRequired"));

            var userExists = await _userRepo.GetByEmail(request.Email);
            if (userExists == null)
                return AppError.Unauthorized(ErrorMessages.Get("Auth:InvalidCredentials"));

            return null;
        }

        public async Task<AppError?> ValidateRefreshTokenAsync(RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return AppError.Validation(ErrorMessages.Get("TokenRequired"));

            return null;
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

        public async Task<(bool Success, string Message)> VerifyEmailAsync(string token)
        {
            var user = await _userRepo.GetByVerificationToken(token);
            if (user == null)
                return (false, ErrorMessages.Get("Auth:InvalidToken"));

            user.IsEmailVerified = true;
            user.RequiresEmailVerification = false;
            user.EmailVerificationToken = null;
            user.EmailVerifiedAt = DateTime.UtcNow;

            await _userRepo.Update(user);

            return (true, SuccessMessages.Get("Email:EmailVerified"));
        }
    }
}

