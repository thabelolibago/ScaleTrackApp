using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.Auth.Services.Shared.Token
{
    public interface ITokenService
    {
        Task<(string AccessToken, string RefreshToken)> CreateTokensAsync(User user);
    }
}
