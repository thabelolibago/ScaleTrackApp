using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Repositories;

namespace ScaleTrackAPI.Services.Auth
{
    public class AuthBusinessRules
    {
        private readonly IUserRepository _userRepo;

        public AuthBusinessRules(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<AppError?> ValidateLoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return AppError.Validation("Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                return AppError.Validation("Password is required.");

            var userExists = await _userRepo.GetByEmail(request.Email);
            if (userExists == null)
                return AppError.Unauthorized("Invalid credentials.");

            return null;
        }

        public async Task<AppError?> ValidateRefreshTokenAsync(RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return AppError.Validation("Refresh token is required.");

            return null;
        }
    }
}
