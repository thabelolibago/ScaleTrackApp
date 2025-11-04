using System.Security.Claims;
using ScaleTrackAPI.Database;
using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Messages;
using ScaleTrackAPI.Services;

public class PasswordResetService : TransactionalServiceBase
{
    private readonly PasswordResetBusinessRules _businessRules;
    private readonly PasswordResetTokenService _tokenService;
    private readonly UserPasswordService _passwordService;
    private readonly EmailHelper _emailHelper;
    private readonly PasswordResetAuditTrail _auditTrail;
    private readonly IConfiguration _config;

    public PasswordResetService(
        PasswordResetBusinessRules businessRules,
        PasswordResetTokenService tokenService,
        UserPasswordService passwordService,
        EmailHelper emailHelper,
        PasswordResetAuditTrail auditTrail,
        IConfiguration config,
        AppDbContext context
    ) : base(context)
    {
        _businessRules = businessRules;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _emailHelper = emailHelper;
        _auditTrail = auditTrail;
        _config = config;
    }

    /// <summary>
    /// Sends a password reset email with a secure token.
    /// </summary>
    public async Task<ResetPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        // Validate user exists
        var validation = await _businessRules.ValidateForgotPasswordAsync(request.Email);
        if (!validation.IsValid)
            return PasswordResetMapper.ToResponse(false, validation.Error!.Message);

        var user = validation.User!;

        // Generate token (valid 1 hour)
        var tokenEntry = await _tokenService.GenerateTokenAsync(user.Id, TimeSpan.FromHours(1));

        // Build reset link
        var frontendUrl = _config["Frontend:BaseUrl"];
        var resetLink = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(tokenEntry.Token)}";

        // Build email and send
        var emailBody = _emailHelper.BuildPasswordResetEmail(user, resetLink);
        await _emailHelper.SendEmailAsync(user.Email, "Password Reset Request", emailBody);

        // Audit password reset request
        await _auditTrail.RecordResetLinkSent(user.Email);

        return PasswordResetMapper.ToResponse(true, SuccessMessages.Get("PasswordReset:ResetLinkSent"));
    }

    /// <summary>
    /// Resets the password using a valid token.
    /// </summary>
    public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request, ClaimsPrincipal currentUser)
    {
        // Decode token from URL
        var decodedToken = Uri.UnescapeDataString(request.Token);

        // Validate token and password
        var validation = await _businessRules.ValidateResetPasswordAsync(
            decodedToken, request.NewPassword, request.ConfirmPassword);

        if (!validation.IsValid)
            return PasswordResetMapper.ToResponse(false, validation.Error!.Message);

        var tokenEntry = validation.Token!;
        var user = tokenEntry.User!;

        // Execute password reset in transaction
        await ExecuteInTransactionAsync(async () =>
        {
            // Update password securely
            await _passwordService.ChangePasswordAsync(user, request.NewPassword);

            // Remove token after successful reset
            await _tokenService.RemoveTokenAsync(tokenEntry);

            // Audit password reset
            await _auditTrail.RecordReset(user.Id, currentUser, user.Email);
        });

        return PasswordResetMapper.ToResponse(true, SuccessMessages.Get("PasswordReset:ResetSuccessful"));
    }
}
