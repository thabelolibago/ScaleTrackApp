using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IPasswordResetRepository;

namespace ScaleTrackAPI.Application.Features.Auth.Password.ForgotPassword.BusinessRules.ForgotPasswordBusinessRules
{
    public class ForgotPasswordBusinessRules
    {
        private readonly IPasswordResetRepository _repo;

        public ForgotPasswordBusinessRules(IPasswordResetRepository repo)
        {
            _repo = repo;
        }

        public async Task<(bool IsValid, AppError? Error, User? User)> ValidateAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, AppError.Validation(ErrorMessages.Get("ForgotPassword:EmailRequired")), null);

            var user = await _repo.GetUserByEmailAsync(email);
            if (user == null)
                return (false, AppError.NotFound(ErrorMessages.Get("User:UserNotFound")), null);

            return (true, null, user);
        }
    }
}
