using System.Security.Claims;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Features.Auth.VerifyEmail.BusinessRules;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Services.Base;
using ScaleTrackAPI.Application.Features.Auth.Shared.AuditTrail;

namespace ScaleTrackAPI.Application.Features.Auth.VerifyEmail.Services
{
    public class VerifyEmailService : TransactionalServiceBase, IVerifyEmailService
    {
        private readonly VerifyEmailBusinessRules _rules;
        private readonly AuthAuditTrail _authAuditTrail;

        public VerifyEmailService(
            AppDbContext context,
            VerifyEmailBusinessRules rules,
            AuthAuditTrail authAuditTrail
        ) : base(context)
        {
            _rules = rules;
            _authAuditTrail = authAuditTrail;
        }

        public async Task<AppError?> VerifyEmailAsync(string token, ClaimsPrincipal? actor = null)
        {
            return await ExecuteInTransactionAsync<AppError?>(async () =>
            {
                var (success, message, user) = await _rules.VerifyEmailAsync(token);

                if (!success)
                    return AppError.Validation(message);

                await _authAuditTrail.RecordVerifyEmailAsync(user!, actor);

                return null;
            });
        }
    }
}
