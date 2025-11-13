using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Auth.Login.BusinessRules;
using ScaleTrackAPI.Application.Features.Auth.Login.DTOs;
using ScaleTrackAPI.Application.Features.Auth.Login.Mappers;
using ScaleTrackAPI.Application.Features.Auth.Refresh.DTOs;
using ScaleTrackAPI.Application.Features.Auth.Refresh.Services;
using ScaleTrackAPI.Application.Features.Auth.Services.Shared.Token;
using ScaleTrackAPI.Application.Features.Auth.Shared.AuditTrail;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Services.Base;
using ScaleTrackAPI.Shared.Helpers;

namespace ScaleTrackAPI.Application.Features.Auth.Login.Services
{
    public class LoginService : TransactionalServiceBase, ILoginService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITokenService _tokenService;
        private readonly PasswordHelper _passwordHelper;
        private readonly LoginBusinessRules _rules;
        private readonly AuthAuditTrail _authAuditTrail;

        public LoginService(
            AppDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IRefreshTokenService refreshTokenService,
            ITokenService tokenService,
            PasswordHelper passwordHelper,
            LoginBusinessRules rules,
            AuthAuditTrail authAuditTrail
        ) : base(context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _refreshTokenService = refreshTokenService;
            _tokenService = tokenService;
            _passwordHelper = passwordHelper;
            _rules = rules;
            _authAuditTrail = authAuditTrail;
        }

        public async Task<(LoginResponse? Entity, AppError? Error)> LoginAsync(LoginRequest request, ClaimsPrincipal actor)
        {
            var validation = await _rules.ValidateLoginAsync(request);
            if (validation != null) return (null, validation);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return (null, AppError.Unauthorized(ErrorMessages.Get("Auth:InvalidCredentials")));

            var passwordWithPepper = _passwordHelper.WithPepper(request.Password);
            var result = await _signInManager.CheckPasswordSignInAsync(user, passwordWithPepper, false);
            if (!result.Succeeded)
                return (null, AppError.Unauthorized(ErrorMessages.Get("Auth:InvalidCredentials")));

            if (user.RequiresEmailVerification && !user.IsEmailVerified)
                return (null, AppError.Unauthorized(ErrorMessages.Get("Email:EmailNotVerified")));

            await _authAuditTrail.RecordLoginAsync(user, actor);

            var response = await GenerateLoginResponseAsync(user);
            return (response, null);
        }

        public async Task<(LoginResponse? Entity, AppError? Error)> RefreshTokenAsync(RefreshTokenRequest request, ClaimsPrincipal actor)
        {
            var validation = await _rules.ValidateRefreshTokenAsync(request);
            if (validation != null) return (null, validation);

            return await ExecuteInTransactionAsync<(LoginResponse? Entity, AppError? Error)>(async () =>
            {
                var storedToken = await _refreshTokenService.GetRefreshTokenAsync(request.RefreshToken);

                if (storedToken == null || storedToken.IsRevoked || storedToken.IsUsed || storedToken.Expires < DateTime.UtcNow)
                    return (null, AppError.Unauthorized(ErrorMessages.Get("Token:TokenExpired")));

                await _refreshTokenService.MarkRefreshTokenUsedAsync(storedToken);

                var user = storedToken.User!;
                await _authAuditTrail.RecordTokenRefreshAsync(user, actor);

                var response = await GenerateLoginResponseAsync(user);
                return (response, null);
            });
        }

        public async Task<LoginResponse> GenerateLoginResponseAsync(User user)
        {
            var (accessToken, refreshToken) = await _tokenService.CreateTokensAsync(user);
            return user.ToLoginResponse(accessToken, refreshToken);
        }

    }
}
