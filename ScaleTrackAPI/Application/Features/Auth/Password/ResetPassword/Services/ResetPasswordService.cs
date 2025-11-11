using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Auth.DTOs.Password;
using ScaleTrackAPI.Application.Features.Auth.Mappers.Password.PasswordResetMapper;
using ScaleTrackAPI.Application.Features.Auth.Password.ResetPassword.BusinessRules.ResetPasswordBusinessRules;
using ScaleTrackAPI.Application.Features.Auth.Password.Shared.PasswordAuditTrail;
using ScaleTrackAPI.Application.Features.Auth.Password.Shared.PasswordResetTokenService;
using ScaleTrackAPI.Application.Features.Auth.Password.Shared.UserPasswordService;
using ScaleTrackAPI.Application.Messages.SuccessMessages;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Services.Base;
using ResetPasswordRequest = ScaleTrackAPI.Application.Features.Auth.DTOs.Password.ResetPasswordRequest;

namespace ScaleTrackAPI.Application.Features.Auth.Password.ResetPassword.Services.ResetPasswordService
{
    /// <summary>
    /// Handles resetting a password using a valid token.
    /// </summary>
    public class ResetPasswordService : TransactionalServiceBase
    {
        private readonly ResetPasswordBusinessRules _businessRules;
        private readonly PasswordResetTokenService _tokenService;
        private readonly UserPasswordService _passwordService;
        private readonly PasswordAuditTrail _auditTrail;

        public ResetPasswordService(
            ResetPasswordBusinessRules businessRules,
            PasswordResetTokenService tokenService,
            UserPasswordService passwordService,
            PasswordAuditTrail auditTrail,
            AppDbContext context) : base(context)
        {
            _businessRules = businessRules;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _auditTrail = auditTrail;
        }

        /// <summary>
        /// Resets the password using a valid token.
        /// </summary>
        public async Task<ResetPasswordResponse> ResetPasswordAsync(
            ResetPasswordRequest request, ClaimsPrincipal currentUser)
        {
            var decodedToken = Uri.UnescapeDataString(request.Token);

            // Validate token and new password
            var validation = await _businessRules.ValidateResetAsync(
                decodedToken, request.NewPassword, request.ConfirmPassword);

            if (!validation.IsValid)
                return PasswordResetMapper.ToResponse(false, validation.Error!.Message);

            var tokenEntry = validation.Token!;
            var user = tokenEntry.User!;

            // Execute within transaction
            await ExecuteInTransactionAsync(async () =>
            {
                // Update password securely
                await _passwordService.ChangePasswordAsync(user, request.NewPassword);

                // Remove token after successful reset
                await _tokenService.RemoveTokenAsync(tokenEntry);

                // Audit password reset
                await _auditTrail.RecordPasswordAction(user.Id, currentUser, user.Email, "Reset");
            });

            return PasswordResetMapper.ToResponse(true, SuccessMessages.Get("ResetPassword:PasswordResetSuccessfully"));
        }
    }
}
