using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Auth.VerifyEmail.Services;
using ScaleTrackAPI.Application.Features.Users.BusinessRules.UserAuditTrail;
using ScaleTrackAPI.Application.Features.Users.DTOs;
using ScaleTrackAPI.Application.Features.Users.Mappers.UserMapper;
using ScaleTrackAPI.Application.Messages.SuccessMessages;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Domain.Enums;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IUserRepository;

namespace ScaleTrackAPI.Application.Features.Users.Services.UserService
{
    public class UserService
    {
        private readonly IUserRepository _repo;
        private readonly UserManager<User> _userManager;
        private readonly UserAuditTrail _auditHelper;
        private readonly IVerifyEmailService _verifyEmailService;
    

        public UserService(
            IUserRepository repo,
            UserManager<User> userManager,
            UserAuditTrail auditHelper,
            IVerifyEmailService verifyEmailService
           
        )
        {
            _repo = repo;
            _userManager = userManager;
            _auditHelper = auditHelper;
            _verifyEmailService = verifyEmailService;
        }

        public async Task<List<UserResponse>> GetAllUsers()
            => (await _repo.GetAll()).Select(UserMapper.ToResponse).ToList();

        public async Task<(UserResponse? Response, AppError? Error)> GetById(int id)
        {
            var u = await _repo.GetById(id);
            if (u == null) return (null, AppError.NotFound(ErrorMessages.Get("User:UserNotFound", id)));
            return (UserMapper.ToResponse(u), null);
        }

        public async Task<(bool Success, AppError? Error)> VerifyEmail(string token)
        {
            var result = await _verifyEmailService.VerifyEmailAsync(token);

            if (result != null)
                return (false, AppError.Validation(ErrorMessages.Get("Auth:InvalidToken")));

            return (true, null);
        }

        public async Task<(AppError? Error, string Message)> UpdateUserRole(int id, int roleIndex, ClaimsPrincipal userClaims)
        {
            if (!Enum.IsDefined(typeof(UserRole), roleIndex))
                return (AppError.Validation(ErrorMessages.Get("User:InvalidRole")), string.Empty);

            var parsedRole = (UserRole)roleIndex;
            var user = await _repo.GetById(id);
            if (user == null)
                return (AppError.NotFound(ErrorMessages.Get("User:UserNotFound")), string.Empty);

            var oldUser = new User
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            };

            user.Role = parsedRole;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return (AppError.Conflict(ErrorMessages.Get("User:UserRoleNotUpdated")), string.Empty);

            await _auditHelper.RecordUpdate(oldUser, user, userClaims);

            return (null, SuccessMessages.Get("User:UserUpdated"));
        }

        public async Task<(AppError? Error, string? Message)> DeleteUser(int id, ClaimsPrincipal userClaims)
        {
            var user = await _repo.GetById(id);
            if (user == null) return (AppError.NotFound(ErrorMessages.Get("User:UserNotFound")), null);

            var identityUser = await _userManager.FindByIdAsync(id.ToString());
            if (identityUser != null)
            {
                var res = await _userManager.DeleteAsync(identityUser);
                if (!res.Succeeded) return (AppError.Conflict(ErrorMessages.Get("User:FailedToDeleteUser")), null);
            }
            else
            {
                await _repo.Delete(user);
            }

            await _auditHelper.RecordDelete(user, userClaims);

            return (null, SuccessMessages.Get("User:UserDeleted"));
        }
    }
}
