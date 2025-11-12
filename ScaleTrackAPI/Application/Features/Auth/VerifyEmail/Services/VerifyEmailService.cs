using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Features.Auth.VerifyEmail.BusinessRules;

namespace ScaleTrackAPI.Application.Features.Auth.VerifyEmail.Services
{
    public class VerifyEmailService
    {
        private readonly VerifyEmailBusinessRules _rules;
        public VerifyEmailService(VerifyEmailBusinessRules rules)
        {
            _rules = rules;
        }

        public async Task<AppError?> VerifyEmailAsync(string token)
        {
            var (success, message) = await _rules.VerifyEmailAsync(token);

            if (!success)
                return AppError.Validation(message);

            return null;
        }
    }
}