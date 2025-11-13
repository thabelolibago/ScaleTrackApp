using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Services.Base;

namespace ScaleTrackAPI.Application.Features.Auth.Services.AuthService
{
    public class AuthService : TransactionalServiceBase, IAuthService
    {
        
        private readonly ITokenService _tokenService;
    
        public AuthService(
            AppDbContext context,
            ITokenService tokenService
           
        ) : base(context)
        {
            _tokenService = tokenService;
        
        }

        
        public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user)
            => await _tokenService.CreateTokensAsync(user);

        
    }
}


