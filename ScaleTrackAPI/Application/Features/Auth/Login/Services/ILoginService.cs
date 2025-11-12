using System.Security.Claims;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Features.Auth.DTOs.Token;
using ScaleTrackAPI.Application.Features.Auth.Login.DTOs;
using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.Auth.Login.Services
{
    public interface ILoginService
    {
        Task<(LoginResponse? Entity, AppError? Error)> LoginAsync(LoginRequest request, ClaimsPrincipal actor);
        Task<(LoginResponse? Entity, AppError? Error)> RefreshTokenAsync(RefreshTokenRequest request, ClaimsPrincipal actor);
        Task<LoginResponse> GenerateLoginResponseAsync(User user);
    }
}