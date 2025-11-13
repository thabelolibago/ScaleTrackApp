using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Auth.RegisterUser.BusinessRules;
using ScaleTrackAPI.Application.Features.Auth.RegisterUser.DTOs;
using ScaleTrackAPI.Application.Features.Auth.Services.Shared.Token;
using ScaleTrackAPI.Application.Features.Users.Mappers.UserMapper;

namespace ScaleTrackAPI.Application.Features.RegisterUser
{
    public class RegisterUserService
    {
       
        private readonly ITokenService _tokenService;
        private readonly RegisterUserBusinessRules _registerUserBusinessRules;
        private readonly RegisterUserAuditTrail _auditHelper;

        public RegisterUserService(
            ITokenService tokenService,
            RegisterUserBusinessRules registerUserBusinessRules,
            RegisterUserAuditTrail auditHelper)
        {
            
            _tokenService = tokenService;
            _registerUserBusinessRules = registerUserBusinessRules;
            _auditHelper = auditHelper;

        }

        public async Task<(RegisterUserResponse? Response, AppError? Error)> RegisterUser(RegisterUserRequest request, string baseUrl)
        {
            if (request == null)
                return (null, AppError.Validation(ErrorMessages.Get("Validation:RequestNotNull")));

            if (!string.Equals(request.Email?.Trim(), request.ConfirmEmail?.Trim(), StringComparison.OrdinalIgnoreCase))
                return (null, AppError.Validation(ErrorMessages.Get("Validation:EmailsDoNotMatch")));

            if (!string.Equals(request.Password, request.ConfirmPassword))
                return (null, AppError.Validation(ErrorMessages.Get("Validation:PasswordsDoNotMatch")));

            // Delegate to business rules
            var (error, user, verificationPending, expiresAt) =
                await _registerUserBusinessRules.RegisterUserRules(request, baseUrl);

            if (error != null)
                return (null, error);

            await _auditHelper.RecordCreate(user!);

            // Generate tokens
            var (accessToken, refreshToken) = await _tokenService.CreateTokensAsync(user!);

            return (new RegisterUserResponse
            {
                User = UserMapper.ToResponse(user!),
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                VerificationPending = verificationPending,
                VerificationExpiresAt = expiresAt
            }, null);
        }
    }
}
