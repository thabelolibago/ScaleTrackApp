using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Models;
using ScaleTrackAPI.Database;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Helpers;
using System.Security.Claims;

namespace ScaleTrackAPI.Services.Auth
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

        // 🔹 Login with audit & business rules
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

            await _auditTrail.RecordLogin(user, actor);

            var response = await GenerateLoginResponseAsync(user);
            return (response, null);
        }

        // 🔹 Refresh token with audit & business rules
        public async Task<(LoginResponse? Entity, AppError? Error)> RefreshTokenAsync(RefreshTokenRequest request, ClaimsPrincipal actor)
        {
            var validation = await _rules.ValidateRefreshTokenAsync(request);
            if (validation != null) return (null, validation);

            return await ExecuteInTransactionAsync<(LoginResponse? Entity, AppError? Error)>(async () =>
            {
                var storedToken = await _tokenService.GetRefreshTokenAsync(request.RefreshToken);

                if (storedToken == null || storedToken.IsRevoked || storedToken.IsUsed || storedToken.Expires < DateTime.UtcNow)
                    return (null, AppError.Unauthorized(ErrorMessages.Get("Token:TokenExpired")));

                await _tokenService.MarkRefreshTokenUsedAsync(storedToken);

                var user = storedToken.User!;
                await _auditTrail.RecordTokenRefresh(user, actor);

                var response = await GenerateLoginResponseAsync(user);
                return (response, null);
            });
        }

        // 🔹 Logout with audit
        public async Task<AppError?> LogoutAsync(LogoutRequest request, ClaimsPrincipal actor)
        {
            var stored = await _tokenService.GetRefreshTokenAsync(request.RefreshToken);
            if (stored == null)
                return AppError.NotFound(ErrorMessages.Get("Token:InvalidToken"));

            await _tokenService.MarkRefreshTokenRevokedAsync(stored);
            await _auditTrail.RecordLogout(stored.User!, actor);

            return null;
        }

        // 🔹 Generate access + refresh tokens
        public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user)
        {
            return await _tokenService.CreateTokensAsync(user);
        }

        // 🔹 Helper to generate login response
        private async Task<LoginResponse> GenerateLoginResponseAsync(User user)
        {
            var (accessToken, refreshToken) = await _tokenService.CreateTokensAsync(user);
            return user.ToLoginResponse(accessToken, refreshToken);
        }
    }
}


