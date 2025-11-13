using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Auth.Login.DTOs;
using ScaleTrackAPI.Application.Features.Auth.Refresh.DTOs;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IUserRepository;

namespace ScaleTrackAPI.Application.Features.Auth.Login.BusinessRules
{
    public class LoginBusinessRules
    {
        private readonly IUserRepository _userRepo;

        public LoginBusinessRules(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<AppError?> ValidateLoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return AppError.Validation(ErrorMessages.Get("Validation:EmailRequired"));

            if (string.IsNullOrWhiteSpace(request.Password))
                return AppError.Validation(ErrorMessages.Get("Validation:PasswordRequired"));

            var userExists = await _userRepo.GetByEmail(request.Email);
            if (userExists == null)
                return AppError.Unauthorized(ErrorMessages.Get("Auth:InvalidCredentials"));

            return null;
        }

        public async Task<AppError?> ValidateRefreshTokenAsync(RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return AppError.Validation(ErrorMessages.Get("TokenRequired"));

            return null;
        }
    }
}