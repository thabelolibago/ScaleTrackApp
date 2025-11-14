using System.Security.Claims;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Features.Auth.VerifyEmail.BusinessRules;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Services.Base;
using ScaleTrackAPI.Application.Features.Auth.Shared.AuditTrail;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IUserRepository;

namespace ScaleTrackAPI.Application.Features.Auth.VerifyEmail.Services
{
    public class VerifyEmailService : TransactionalServiceBase, IVerifyEmailService
    {
        private readonly VerifyEmailBusinessRules _rules;
        private readonly AuthAuditTrail _authAuditTrail;
        private readonly IUserRepository _repo;

        public VerifyEmailService(
            AppDbContext context,
            VerifyEmailBusinessRules rules,
            AuthAuditTrail authAuditTrail,
            IUserRepository repo    
        ) : base(context)
        {
            _rules = rules;
            _authAuditTrail = authAuditTrail;
            _repo = repo;
        }

        public async Task<AppError?> VerifyEmailAsync(string token)
        {
            var user = await _repo.GetByVerificationToken(token);

            if (user == null)
                return AppError.NotFound("Invalid or expired verification token.");

            if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
                return AppError.Expired("Verification link has expired.");

            // Mark verified
            user.IsEmailVerified = true;
            user.RequiresEmailVerification = false;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;

            await _repo.Update(user);

            await _authAuditTrail.RecordVerifyEmailAsync(user);

            return null;
        }

    }
}
