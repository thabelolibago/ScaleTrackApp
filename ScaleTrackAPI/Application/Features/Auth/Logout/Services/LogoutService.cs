using System.Security.Claims;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Auth.Logout.DTOs;
using ScaleTrackAPI.Application.Features.Auth.Refresh.Services;
using ScaleTrackAPI.Application.Features.Auth.Shared.AuditTrail;


namespace ScaleTrackAPI.Application.Features.Auth.Logout.Services
{
    public class LogoutService : ILogoutService
    {
       
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly AuthAuditTrail _authAuditTrail;
        public LogoutService(AuthAuditTrail auditTrail, IRefreshTokenService refreshTokenService)
        {
            
            _refreshTokenService = refreshTokenService;
            _authAuditTrail = auditTrail;

        }

        public async Task<AppError?> LogoutAsync(LogoutRequest request, ClaimsPrincipal actor)
        {
            var stored = await _refreshTokenService.GetRefreshTokenAsync(request.RefreshToken);
            if (stored == null)
                return AppError.NotFound(ErrorMessages.Get("Auth:InvalidRefreshToken"));

            await _refreshTokenService.MarkRefreshTokenRevokedAsync(stored);
            await _authAuditTrail.RecordLogoutAsync(stored.User!, actor);

            return null;
        }
    }
}