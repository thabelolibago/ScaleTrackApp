using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Auth.RegisterUser.DTOs;
using ScaleTrackAPI.Application.Features.Auth.RegisterUser.Mappers;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Domain.Enums;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IUserRepository;
using ScaleTrackAPI.Shared.Helpers;

namespace ScaleTrackAPI.Application.Features.Auth.RegisterUser.BusinessRules
{
    public class RegisterUserBusinessRules
    {
        private readonly UserManager<User> _userManager;
        private readonly PasswordHelper _passwordHelper;
        private readonly IUserRepository _repo;
        private readonly EmailHelper _emailHelper;


        public RegisterUserBusinessRules(
            UserManager<User> userManager,
            PasswordHelper passwordHelper,
            IUserRepository repo,
            EmailHelper emailHelper
        )
        {
            _userManager = userManager;
            _passwordHelper = passwordHelper;
            _repo = repo;
            _emailHelper = emailHelper;
        }

        public async Task<(AppError? Error, User? User, bool VerificationPending, DateTime? ExpiresAt)>
    RegisterUserRules(RegisterUserRequest request, string baseUrl)
        {
            if (request == null)
                return (AppError.Validation(ErrorMessages.Get("Validation:RequestNotNull")), null, false, null);

            // Check if a user with this email already exists
            var existingUser = await _repo.GetByEmail(request.Email);
            if (existingUser != null)
            {
                if (!existingUser.IsEmailVerified)
                    return (AppError.Conflict(ErrorMessages.Get("User:EmailNotVerified")), null, false, null);

                var message = string.Format(ErrorMessages.Get("User:EmailAlreadyExists"), request.Email);
                return (AppError.Conflict(message), null, false, null);
            }

            // Create new user model
            var user = RegisterUserMapper.ToModel(request);
            user.Role = UserRole.Viewer;
            user.IsEmailVerified = false;
            user.RequiresEmailVerification = true;

            // Hash password with pepper
            var password = _passwordHelper.WithPepper(request.Password);
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return (AppError.Validation(errors), null, false, null);
            }

            // Add role claim
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, user.Role.ToString()));

            // Email verification
            DateTime? verificationExpiresAt = DateTime.UtcNow.AddHours(24);
            user.EmailVerificationToken = Guid.NewGuid().ToString("N");
            user.EmailVerificationTokenExpiry = verificationExpiresAt;
            await _repo.Update(user);

            string verifyLink = $"{baseUrl}/verify-email?token={user.EmailVerificationToken}";
            string emailBody = _emailHelper.BuildEmailVerificationEmail(user, verifyLink);
            await _emailHelper.SendEmailAsync(user.Email!, "Verify Your Email", emailBody);

            return (null, user, true, verificationExpiresAt);
        }

    }
}