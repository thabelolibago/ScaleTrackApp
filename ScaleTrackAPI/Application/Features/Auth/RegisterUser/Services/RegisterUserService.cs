using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Auth.RegisterUser.BusinessRules;
using ScaleTrackAPI.Application.Features.Auth.RegisterUser.DTOs;
using ScaleTrackAPI.Application.Features.Auth.Shared.AuditTrail;
using ScaleTrackAPI.Application.Features.Users.Mappers.UserMapper;
using ScaleTrackAPI.Application.Messages.SuccessMessages;

namespace ScaleTrackAPI.Application.Features.RegisterUser
{
    /// <summary>
    /// Handles user registration and enforces email verification before login.
    /// </summary>
    public class RegisterUserService
    {
        private readonly RegisterUserBusinessRules _registerUserBusinessRules;
        private readonly AuthAuditTrail _auditHelper;

        public RegisterUserService(
            RegisterUserBusinessRules registerUserBusinessRules,
            AuthAuditTrail auditHelper)
        {
            _registerUserBusinessRules = registerUserBusinessRules;
            _auditHelper = auditHelper;
        }

        /// <summary>
        /// Registers a new user and sends an email verification link.
        /// Login is blocked until verification occurs.
        /// </summary>
        public async Task<(RegisterUserResponse? Response, AppError? Error)> RegisterUser(RegisterUserRequest request, string baseUrl)
        {
            if (request == null)
                return (null, AppError.Validation(ErrorMessages.Get("Validation:RequestNotNull")));

            if (!string.Equals(request.Email?.Trim(), request.ConfirmEmail?.Trim(), StringComparison.OrdinalIgnoreCase))
                return (null, AppError.Validation(ErrorMessages.Get("Validation:EmailsDoNotMatch")));

            if (!string.Equals(request.Password, request.ConfirmPassword))
                return (null, AppError.Validation(ErrorMessages.Get("Validation:PasswordsDoNotMatch")));

            // Business validation + user creation
            var (error, user, verificationPending, expiresAt) =
                await _registerUserBusinessRules.RegisterUserRules(request, baseUrl);

            if (error != null)
                return (null, error);

            await _auditHelper.RecordRegisterAsync(user!);

            // 🚫 Tokens are NOT generated until email verification succeeds
            return (new RegisterUserResponse
            {
                Message = SuccessMessages.Get("RegisterUser:UserRegisteredVerifyEmail")
            }, null);
        }
    }
}
