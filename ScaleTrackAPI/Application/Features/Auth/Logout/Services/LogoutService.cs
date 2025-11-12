using System.Security.Claims;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Auth.BusinessRules.AuthAuditTrail;
using ScaleTrackAPI.Application.Features.Auth.Logout.DTOs;
using ScaleTrackAPI.Application.Features.Auth.Services;

namespace ScaleTrackAPI.Application.Features.Auth.Logout.Services
{
    public class LogoutService : ILogoutService
    {
        private readonly ITokenService _tokenService;
        private readonly AuthAuditTrail _auditTrail;
        public LogoutService(ITokenService tokenService, AuthAuditTrail auditTrail)
        {
            _tokenService = tokenService;
            _auditTrail = auditTrail;
        }

        public async Task<AppError?> LogoutAsync(LogoutRequest request, ClaimsPrincipal actor)
        {
            var stored = await _tokenService.GetRefreshTokenAsync(request.RefreshToken);
            if (stored == null)
                return AppError.NotFound(ErrorMessages.Get("Auth:InvalidRefreshToken"));

            await _tokenService.MarkRefreshTokenRevokedAsync(stored);
            await _auditTrail.RecordLogout(stored.User!, actor);

            return null;
        }
    }
}