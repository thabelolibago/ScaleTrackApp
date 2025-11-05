using System.Security.Cryptography;
using ScaleTrackAPI.Models;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Repositories;

namespace ScaleTrackAPI.Services.Shared
{
    public class PasswordResetTokenService
    {
        private readonly IPasswordResetRepository _repo;

        public PasswordResetTokenService(IPasswordResetRepository repo)
        {
            _repo = repo;
        }

        public async Task<PasswordResetToken> GenerateTokenAsync(int userId, TimeSpan expiration)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var entity = PasswordResetMapper.ToEntity(userId, token, DateTime.UtcNow.Add(expiration));
            await _repo.AddResetTokenAsync(entity);
            return entity;
        }

        public async Task<PasswordResetToken?> GetTokenAsync(string token) =>
            await _repo.GetTokenAsync(token);

        public async Task RemoveTokenAsync(PasswordResetToken token) =>
            await _repo.RemoveResetTokenAsync(token);
    }
}
