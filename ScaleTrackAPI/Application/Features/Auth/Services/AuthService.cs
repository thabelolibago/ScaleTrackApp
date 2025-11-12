using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Auth.BusinessRules.AuthAuditTrail;
using ScaleTrackAPI.Application.Features.Auth.BusinessRules.AuthBusinessRules;
using ScaleTrackAPI.Application.Features.Auth.DTOs.Logout;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Services.Base;
using ScaleTrackAPI.Shared.Helpers;
using System.Security.Claims;

namespace ScaleTrackAPI.Application.Features.Auth.Services.AuthService
{
    public class AuthService : TransactionalServiceBase, IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly PasswordHelper _passwordHelper;
        private readonly AuthBusinessRules _rules;
        private readonly AuthAuditTrail _auditTrail;

        public AuthService(
            AppDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService,
            PasswordHelper passwordHelper,
            AuthBusinessRules rules,
            AuthAuditTrail auditTrail
        ) : base(context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _passwordHelper = passwordHelper;
            _rules = rules;
            _auditTrail = auditTrail;
        }

    

        public async Task<AppError?> VerifyEmailAsync(string token)
        {
            var (success, message) = await _rules.VerifyEmailAsync(token);

            if (!success)
                return AppError.Validation(message);

            return null;
        }

        public async Task<AppError?> ResendVerificationEmailAsync(string email, string baseUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return AppError.NotFound(ErrorMessages.Get("User:UserNotFound", email));

            if (!user.RequiresEmailVerification)
                return AppError.Validation(ErrorMessages.Get("Validation:InvalidEmailVerificationRequest"));

            if (user.IsEmailVerified)
                return AppError.Conflict(ErrorMessages.Get("User:EmailAlreadyExists", user.Email!));

            await _rules.GenerateEmailVerificationAsync(user, baseUrl);
            return null;
        }

        public async Task<AppError?> LogoutAsync(LogoutRequest request, ClaimsPrincipal actor)
        {
            var stored = await _tokenService.GetRefreshTokenAsync(request.RefreshToken);
            if (stored == null)
                return AppError.NotFound(ErrorMessages.Get("Auth:InvalidRefreshToken"));

            await _tokenService.MarkRefreshTokenRevokedAsync(stored);
            await _auditTrail.RecordLogout(stored.User!, actor);

            return null;
        }

        public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user)
            => await _tokenService.CreateTokensAsync(user);

        
    }
}


