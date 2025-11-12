using System.Security.Claims;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Features.Auth.Logout.DTOs;

namespace ScaleTrackAPI.Application.Features.Auth.Logout.Services
{
    public interface ILogoutService
    {
        Task<AppError?> LogoutAsync(LogoutRequest request, ClaimsPrincipal actor);
    }
}