using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.Services.Shared;
using ScaleTrackAPI.Database;
using Microsoft.Extensions.Configuration;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Helpers;

namespace ScaleTrackAPI.Services.ForgotPassword
{
    public class ForgotPasswordService : TransactionalServiceBase
    {
        private readonly ForgotPasswordBusinessRules _businessRules;
        private readonly PasswordResetTokenService _tokenService;
        private readonly EmailHelper _emailHelper;
        private readonly PasswordAuditTrail _auditTrail;
        private readonly IConfiguration _config;

        public ForgotPasswordService(
            ForgotPasswordBusinessRules businessRules,
            PasswordResetTokenService tokenService,
            EmailHelper emailHelper,
            PasswordAuditTrail auditTrail,
            IConfiguration config,
            AppDbContext context) : base(context)
        {
            _businessRules = businessRules;
            _tokenService = tokenService;
            _emailHelper = emailHelper;
            _auditTrail = auditTrail;
            _config = config;
        }

        public async Task<ResetPasswordResponse> SendResetLinkAsync(ForgotPasswordRequest request)
        {
            var validation = await _businessRules.ValidateAsync(request.Email);
            if (!validation.IsValid)
                return PasswordResetMapper.ToResponse(false, validation.Error!.Message);

            var user = validation.User!;
            var tokenEntry = await _tokenService.GenerateTokenAsync(user.Id, TimeSpan.FromHours(1));

            var frontendUrl = _config["Frontend:BaseUrl"];
            var resetLink = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(tokenEntry.Token)}";

            var emailBody = _emailHelper.BuildPasswordResetEmail(user, resetLink);
            await _emailHelper.SendEmailAsync(user.Email, "Password Reset Request", emailBody);

            await _auditTrail.RecordResetLinkSent(user.Email);

            return PasswordResetMapper.ToResponse(true, "Reset link sent successfully.");
        }
    }
}
