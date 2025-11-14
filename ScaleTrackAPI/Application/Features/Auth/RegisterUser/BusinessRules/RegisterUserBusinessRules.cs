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
    /// <summary>
    /// Encapsulates all registration validation rules and user creation workflow.
    /// Ensures strong security by preventing login until email verification is completed.
    /// </summary>
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

        /// <summary>
        /// Executes the full user registration rule pipeline:
        /// 1. Validates input
        /// 2. Ensures email uniqueness
        /// 3. Creates user
        /// 4. Assigns default role
        /// 5. Generates email verification token (required before login)
        ///
        /// Returns:
        /// - Error (if any)
        /// - Created User
        /// - VerificationPending = true (always for new users)
        /// - Token expiration timestamp
        /// </summary>
        public async Task<(AppError? Error, User? User, bool VerificationPending, DateTime? ExpiresAt)>
            RegisterUserRules(RegisterUserRequest request, string baseUrl)
        {
            // ------------------------------
            // 1. Validate request
            // ------------------------------
            if (request == null)
                return (AppError.Validation(ErrorMessages.Get("Validation:RequestNotNull")),
                        null, false, null);

            // ------------------------------
            // 2. Ensure email is not in use
            // ------------------------------
            var existingUser = await _repo.GetByEmail(request.Email);
            if (existingUser != null)
            {
                // User exists but email not verified → force verification
                if (!existingUser.IsEmailVerified)
                {
                    return (AppError.Conflict(ErrorMessages.Get("User:EmailNotVerified")),
                            null, false, null);
                }

                // Fully registered + verified → reject
                var message = string.Format(
                    ErrorMessages.Get("User:EmailAlreadyExists"),
                    request.Email);

                return (AppError.Conflict(message), null, false, null);
            }

            // ------------------------------
            // 3. Create new user entity
            // ------------------------------
            var user = RegisterUserMapper.ToModel(request);
            user.Role = UserRole.Viewer;
            user.IsEmailVerified = false;             // Not verified yet
            user.RequiresEmailVerification = true;     // BLOCK login until verification

            // ------------------------------
            // 4. Secure password hashing (PBKDF2 + pepper)
            // ------------------------------
            var hashedPassword = _passwordHelper.WithPepper(request.Password);
            var createResult = await _userManager.CreateAsync(user, hashedPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                return (AppError.Validation(errors), null, false, null);
            }

            // Ensure role claim exists
            await _userManager.AddClaimAsync(
                user,
                new Claim(ClaimTypes.Role, user.Role.ToString())
            );

            // ------------------------------
            // 5. Generate verification token
            // ------------------------------
            var expiresAt = DateTime.UtcNow.AddHours(24);

            user.EmailVerificationToken = Guid.NewGuid().ToString("N");
            user.EmailVerificationTokenExpiry = expiresAt;

            await _repo.Update(user);

            // Build verification link
            string verifyLink = $"{baseUrl}/verify-email?token={user.EmailVerificationToken}";
            string emailBody = _emailHelper.BuildEmailVerificationEmail(user, verifyLink);

            // Send verification email
            await _emailHelper.SendEmailAsync(
                user.Email!,
                "Verify Your Email",
                emailBody
            );

            // ------------------------------
            // 6. Return success — user cannot log in yet
            // ------------------------------
            return (null, user, true, expiresAt);
        }
    }
}
