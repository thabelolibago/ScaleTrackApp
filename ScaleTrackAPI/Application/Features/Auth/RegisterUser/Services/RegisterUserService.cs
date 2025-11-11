using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Auth.BusinessRules.AuthBusinessRules;
using ScaleTrackAPI.Application.Features.Auth.RegisterUser.BusinessRules;
using ScaleTrackAPI.Application.Features.Auth.RegisterUser.DTOs;
using ScaleTrackAPI.Application.Features.Auth.RegisterUser.Mappers;
using ScaleTrackAPI.Application.Features.Auth.Services;
using ScaleTrackAPI.Application.Features.Users.Mappers.UserMapper;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Domain.Enums;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IUserRepository;
using ScaleTrackAPI.Shared.Helpers;

namespace ScaleTrackAPI.Application.Features.RegisterUser
{
    public class RegisterUserService
    {
        private readonly IUserRepository _repo;
        private readonly UserManager<User> _userManager;
        private readonly PasswordHelper _passwordHelper;
        private readonly ITokenService _tokenService;
        private readonly AuthBusinessRules _authRules;
        private readonly RegisterUserAuditTrail _auditHelper;
    
        public RegisterUserService(IUserRepository repo,
            UserManager<User> userManager,
            PasswordHelper passwordHelper,
            ITokenService tokenService,
            AuthBusinessRules authRules,
            RegisterUserAuditTrail auditHelper  )
        {
            _repo = repo;
            _userManager = userManager;
            _passwordHelper = passwordHelper;
            _tokenService = tokenService;
            _authRules = authRules;
            _auditHelper = auditHelper;
            
        }

         public async Task<(RegisterUserResponse? Response, AppError? Error)> RegisterUser(RegisterUserRequest request, string baseUrl)
        {
            if (request == null)
                return (null, AppError.Validation(ErrorMessages.Get("Validation:RequestNotNull")));

            var existingUser = await _repo.GetByEmail(request.Email);
            if (existingUser != null)
            {
                if (!existingUser.IsEmailVerified)
                    return (null, AppError.Conflict(ErrorMessages.Get("Validation:EmailsDoNotMatch")));

                return (null, AppError.Conflict(ErrorMessages.Get("User:EmailAlreadyExists")));
            }

            var user = RegisterUserMapper.ToModel(request);
            user.Role = UserRole.Viewer;
            user.IsEmailVerified = false;
            user.RequiresEmailVerification = true;

            var passwordWithPepper = _passwordHelper.WithPepper(request.Password);
            var result = await _userManager.CreateAsync(user, passwordWithPepper);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return (null, AppError.Validation(errors));
            }

            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, user.Role.ToString()));

            await _authRules.GenerateEmailVerificationAsync(user, baseUrl);

            var (accessToken, refreshToken) = await _tokenService.CreateTokensAsync(user);

            var actor = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }, "Registration"));

            await _auditHelper.RecordCreate(user);

            return (new RegisterUserResponse
            {
                User = UserMapper.ToResponse(user),
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }, null);
        }
    }

}
