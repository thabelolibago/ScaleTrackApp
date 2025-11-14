using System.Security.Claims;
using ScaleTrackAPI.Application.Errors.AppError;

namespace ScaleTrackAPI.Application.Features.Auth.VerifyEmail.Services
{
    public interface IVerifyEmailService
    {
        Task<AppError?> VerifyEmailAsync(string token, ClaimsPrincipal? actor = null);
    }
}