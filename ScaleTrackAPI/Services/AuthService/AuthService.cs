using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.Models;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Mappers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ScaleTrackAPI.Database;

namespace ScaleTrackAPI.Services.Auth
{
    public class AuthService : TransactionalServiceBase, IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly IRefreshTokenRepository _refreshTokenRepo;

        public AuthService(
            AppDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration config,
            IRefreshTokenRepository refreshTokenRepo
        ) : base(context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _refreshTokenRepo = refreshTokenRepo;
        }

        public async Task<(LoginResponse? Entity, AppError? Error)> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return (null, AppError.Unauthorized(ErrorMessages.Get("InvalidCredentials")));

            var pepper = _config["Security:PasswordPepper"] ?? "";
            var passwordWithPepper = request.Password + pepper;

            var result = await _signInManager.CheckPasswordSignInAsync(user, passwordWithPepper, false);
            if (!result.Succeeded)
                return (null, AppError.Unauthorized(ErrorMessages.Get("InvalidCredentials")));

            var (accessToken, refreshToken) = await GenerateTokensAsync(user);

            var response = new LoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                User = new UserResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? "",
                    Role = Enum.TryParse<UserRole>(user.Role, out var parsedRole)
                           ? parsedRole
                           : UserRole.Viewer
                }
            };

            return (response, null);
        }

        public async Task<(LoginResponse? Entity, AppError? Error)> RefreshTokenAsync(RefreshTokenRequest request)
        {
            return await ExecuteInTransactionAsync<(LoginResponse? Entity, AppError? Error)>(async () =>
            {
                var storedToken = await _refreshTokenRepo.GetByTokenAsync(request.RefreshToken);

                if (storedToken == null)
                    return (null, AppError.Unauthorized(ErrorMessages.Get("InvalidToken")));

                if (storedToken.IsRevoked || storedToken.IsUsed || storedToken.Expires < DateTime.UtcNow)
                    return (null, AppError.Unauthorized(ErrorMessages.Get("TokenExpired")));

                // mark used
                storedToken.IsUsed = true;
                await _refreshTokenRepo.UpdateAsync(storedToken);

                var user = storedToken.User;
                if (user == null)
                    return (null, AppError.Unauthorized(ErrorMessages.Get("InvalidToken")));

                // generate new tokens
                var (newAccessToken, newRefreshToken) = await GenerateTokensAsync(user);

                var response = new LoginResponse
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    User = new UserResponse
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email ?? "",
                        Role = Enum.TryParse<UserRole>(user.Role, out var parsedRole)
                               ? parsedRole
                               : UserRole.Viewer
                    }
                };

                return (response, null);
            });
        }


        public async Task<AppError?> LogoutAsync(LogoutRequest request)
        {
            var stored = await _refreshTokenRepo.GetByTokenAsync(request.RefreshToken);
            if (stored == null)
                return AppError.NotFound(ErrorMessages.Get("InvalidToken"));

            stored.IsRevoked = true;
            await _refreshTokenRepo.UpdateAsync(stored);
            await _refreshTokenRepo.SaveChangesAsync();

            return null;
        }

        public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user)
        {
            var minutes = int.TryParse(_config["Jwt:AccessTokenMinutes"], out var m) ? m : 60;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = Guid.NewGuid().ToString("N");
            var refreshExpiry = DateTime.UtcNow.AddDays(7);

            var refreshEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                Expires = refreshExpiry,
                IsRevoked = false,
                IsUsed = false
            };

            await _refreshTokenRepo.AddAsync(refreshEntity);
            await _refreshTokenRepo.SaveChangesAsync();

            return (accessToken, refreshToken);
        }
    }
}


