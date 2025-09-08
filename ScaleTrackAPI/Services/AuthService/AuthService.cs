using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.Models;
using ScaleTrackAPI.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ScaleTrackAPI.Services.Auth
{
    public class AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration config,
        IRefreshTokenRepository refreshTokenRepo) : IAuthService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IConfiguration _config = config;
        private readonly IRefreshTokenRepository _refreshTokenRepo = refreshTokenRepo;

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return null;

            var pepper = _config["Security:PasswordPepper"] ?? "";
            var passwordWithPepper = request.Password + pepper;

            var result = await _signInManager.CheckPasswordSignInAsync(user, passwordWithPepper, false);
            if (!result.Succeeded) return null;

            var (accessToken, refreshToken) = await GenerateTokensAsync(user);

            return new LoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                User = new UserResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!,
                    Role = user.Role
                }
            };
        }

        public async Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var storedToken = await _refreshTokenRepo.GetByTokenAsync(request.RefreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.IsUsed || storedToken.Expires < DateTime.UtcNow)
                return null;

            storedToken.IsUsed = true;
            await _refreshTokenRepo.UpdateAsync(storedToken);
            await _refreshTokenRepo.SaveChangesAsync();

            var user = storedToken.User!;
            var (newAccessToken, newRefreshToken) = await GenerateTokensAsync(user);

            return new LoginResponse
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                User = new UserResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!,
                    Role = user.Role
                }
            };
        }

        public Task<bool> LogoutAsync(LogoutRequest request)
        {
            return Task.FromResult(true);
        }

        private async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user)
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


