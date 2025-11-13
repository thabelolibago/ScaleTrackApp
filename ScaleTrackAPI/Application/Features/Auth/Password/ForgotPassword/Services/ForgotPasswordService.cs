using ScaleTrackAPI.Application.Features.Auth.Password.ForgotPassword.BusinessRules.ForgotPasswordBusinessRules;
using ScaleTrackAPI.Application.Features.Auth.Password.ForgotPassword.DTOs;
using ScaleTrackAPI.Application.Features.Auth.Password.Mappers;
using ScaleTrackAPI.Application.Features.Auth.Password.ResetPassword.DTOs;
using ScaleTrackAPI.Application.Features.Auth.Password.Shared.PasswordAuditTrail;
using ScaleTrackAPI.Application.Features.Auth.Password.Shared.PasswordResetTokenService;
using ScaleTrackAPI.Application.Messages.SuccessMessages;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Services.Base;
using ScaleTrackAPI.Shared.Helpers;

namespace ScaleTrackAPI.Application.Features.Auth.Password.ForgotPassword.Services.ForgotPasswordService
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

            return PasswordResetMapper.ToResponse(true, SuccessMessages.Get("ForgotPassword:PasswordResetEmailSent"));
        }
    }
}
