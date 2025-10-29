using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Models;
using ScaleTrackAPI.Database;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Helpers;

namespace ScaleTrackAPI.Services.Auth
{
    public class AuthService : TransactionalServiceBase, IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly PasswordHelper _passwordHelper;

        public AuthService(
            AppDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService,
            PasswordHelper passwordHelper
        ) : base(context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _passwordHelper = passwordHelper;
        }

        public async Task<(LoginResponse? Entity, AppError? Error)> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return (null, AppError.Unauthorized(ErrorMessages.Get("Auth:InvalidCredentials")));

            var passwordWithPepper = _passwordHelper.WithPepper(request.Password);
            var result = await _signInManager.CheckPasswordSignInAsync(user, passwordWithPepper, false);

            if (!result.Succeeded)
                return (null, AppError.Unauthorized(ErrorMessages.Get("Auth:InvalidCredentials")));

            var response = await GenerateLoginResponseAsync(user);
            return (response, null);
        }
        public async Task<(LoginResponse? Entity, AppError? Error)> RefreshTokenAsync(RefreshTokenRequest request)
        {
            return await ExecuteInTransactionAsync<(LoginResponse? Entity, AppError? Error)>(async () =>
            {
                var storedToken = await _tokenService.GetRefreshTokenAsync(request.RefreshToken);

                if (storedToken == null || storedToken.IsRevoked || storedToken.IsUsed || storedToken.Expires < DateTime.UtcNow)
                    return (null, AppError.Unauthorized(ErrorMessages.Get("Token:TokenExpired")));

                await _tokenService.MarkRefreshTokenUsedAsync(storedToken);

                var user = storedToken.User!;
                var response = await GenerateLoginResponseAsync(user);

                return (response, null);
            });
        }

        public async Task<AppError?> LogoutAsync(LogoutRequest request)
        {
            var stored = await _tokenService.GetRefreshTokenAsync(request.RefreshToken);
            if (stored == null)
                return AppError.NotFound(ErrorMessages.Get("Token:InvalidToken"));

            await _tokenService.MarkRefreshTokenRevokedAsync(stored);
            return null;
        }

        public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user)
        {
            return await _tokenService.CreateTokensAsync(user);
        }

        private async Task<LoginResponse> GenerateLoginResponseAsync(User user)
        {
            var (accessToken, refreshToken) = await _tokenService.CreateTokensAsync(user);
            return user.ToLoginResponse(accessToken, refreshToken);
        }
    }
}


