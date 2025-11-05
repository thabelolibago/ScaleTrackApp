using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Services.ForgotPassword
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
                return (false, AppError.Validation("Email is required."), null);

            var user = await _repo.GetUserByEmailAsync(email);
            if (user == null)
                return (false, AppError.NotFound("User not found."), null);

            return (true, null, user);
        }
    }
}
