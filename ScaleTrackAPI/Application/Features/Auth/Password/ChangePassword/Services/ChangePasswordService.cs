using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.Application.Features.Auth.DTOs.Password;
using ScaleTrackAPI.Application.Features.Auth.Mappers.Password.PasswordResetMapper;
using ScaleTrackAPI.Application.Features.Auth.Password.ChangePassword.BusinessRules.ChangePasswordBusinessRules;
using ScaleTrackAPI.Application.Features.Auth.Password.Shared.PasswordAuditTrail;
using ScaleTrackAPI.Application.Features.Auth.Password.Shared.UserPasswordService;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Services.Base;
using ScaleTrackAPI.Shared.Helpers;

namespace ScaleTrackAPI.Application.Features.Auth.Password.ChangePassword.Services.ChangePasswordService
{
    public class ChangePasswordService : TransactionalServiceBase
    {
        private readonly UserPasswordService _passwordService;
        private readonly ChangePasswordBusinessRules _businessRules;
        private readonly PasswordAuditTrail _auditTrail;
        private readonly UserManager<User> _userManager;
        private readonly PasswordHelper _passwordHelper;

        public ChangePasswordService(
            UserPasswordService passwordService,
            ChangePasswordBusinessRules businessRules,
            PasswordAuditTrail auditTrail,
            UserManager<User> userManager,
            PasswordHelper passwordHelper,
            AppDbContext context) : base(context)
        {
            _passwordService = passwordService;
            _businessRules = businessRules;
            _auditTrail = auditTrail;
            _userManager = userManager;
            _passwordHelper = passwordHelper;
        }

        public async Task<ResetPasswordResponse> ChangePasswordAsync(
            int userId, ChangePasswordRequest request, ClaimsPrincipal currentUser)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return PasswordResetMapper.ToResponse(false, "User not found.");

            // ✅ Manually verify current password with pepper
            var isCurrentValid = _passwordHelper.VerifyWithPepper(request.CurrentPassword, user.PasswordHash);
            if (!isCurrentValid)
                return PasswordResetMapper.ToResponse(false, "Incorrect password.");

            // Validate new password
            var validation = await _businessRules.ValidateChangePasswordAsync(
                userId, request.CurrentPassword, request.NewPassword, request.ConfirmPassword);
            if (!validation.IsValid)
                return PasswordResetMapper.ToResponse(false, validation.Error!.Message);

            // Change password securely via shared service
            await _passwordService.ChangePasswordAsync(user, request.NewPassword);

            // Audit the change
            await _auditTrail.RecordPasswordAction(user.Id, currentUser, user.Email, "Change");

            return PasswordResetMapper.ToResponse(true, "Password changed successfully.");
        }
    }
}
