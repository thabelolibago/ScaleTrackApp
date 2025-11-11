using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IRefreshTokenRepository;


namespace ScaleTrackAPI.Application.Features.Auth.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly IRefreshTokenRepository _refreshTokenRepo;

        public TokenService(IConfiguration config, IRefreshTokenRepository refreshTokenRepo)
        {
            _config = config;
            _refreshTokenRepo = refreshTokenRepo;
        }

        public async Task<(string AccessToken, string RefreshToken)> CreateTokensAsync(User user)
        {
            var accessToken = GenerateAccessToken(user);

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

        private string GenerateAccessToken(User user)
        {
            var minutes = int.TryParse(_config["Jwt:AccessTokenMinutes"], out var m) ? m : 60;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _refreshTokenRepo.GetByTokenAsync(token);
        }

        public async Task MarkRefreshTokenUsedAsync(RefreshToken token)
        {
            token.IsUsed = true;
            await _refreshTokenRepo.UpdateAsync(token);
            await _refreshTokenRepo.SaveChangesAsync();
        }

        public async Task MarkRefreshTokenRevokedAsync(RefreshToken token)
        {
            token.IsRevoked = true;
            await _refreshTokenRepo.UpdateAsync(token);
            await _refreshTokenRepo.SaveChangesAsync();
        }
    }
}

