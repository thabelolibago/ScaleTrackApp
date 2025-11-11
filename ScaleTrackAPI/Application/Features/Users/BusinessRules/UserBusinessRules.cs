using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Users.DTOs;
using ScaleTrackAPI.Application.Features.Users.Mappers.UserMapper;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Domain.Enums;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IUserRepository;
using ScaleTrackAPI.Shared.Helpers;
using ScaleTrackAPI.Shared.Validators;
using System.Security.Claims;

namespace ScaleTrackAPI.Application.Features.Users.BusinessRules.UserBusinessRules
{
    public class UserBusinessRules
    {
        private readonly IUserRepository _repo;
        private readonly UserManager<User> _userManager;
        private readonly IValidator<UserRequest> _validator;
        private readonly PasswordHelper _passwordHelper;
        private readonly EmailHelper _emailHelper;

        public UserBusinessRules(
            IUserRepository repo,
            UserManager<User> userManager,
            IValidator<UserRequest> validator,
            PasswordHelper passwordHelper,
            EmailHelper emailHelper
        )
        {
            _repo = repo;
            _userManager = userManager;
            _validator = validator;
            _passwordHelper = passwordHelper;
            _emailHelper = emailHelper;
        }

        
        
        public async Task<(AppError? Error, User? OldUser, User? UpdatedUser)> UpdateUserRoleRules(User user, int roleIndex)
        {
            if (!Enum.IsDefined(typeof(UserRole), roleIndex))
                return (AppError.Validation(ErrorMessages.Get("User:InvalidRole")), null, null);

            var parsedRole = (UserRole)roleIndex;
            if (user.Role == parsedRole)
                return (AppError.Validation(ErrorMessages.Get("User:RoleAlreadyAssignedToUser")), null, null);

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
                return (AppError.Conflict(ErrorMessages.Get("User:UserRoleNotUpdated")), null, null);

            return (null, oldUser, user);
        }

        public async Task<(AppError? Error, User? OldUser)> DeleteUserRules(User user)
        {
            var oldUser = new User
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            };

            var identityUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (identityUser != null)
            {
                var result = await _userManager.DeleteAsync(identityUser);
                if (!result.Succeeded)
                    return (AppError.Conflict(ErrorMessages.Get("User:FailedToDeleteUser")), null);
            }
            else
            {
                await _repo.Delete(user);
            }

            return (null, oldUser);
        }

        public async Task<(AppError? Error, string? Token)> GenerateEmailVerificationAsync(User user, string baseUrl)
        {
            user.EmailVerificationToken = Guid.NewGuid().ToString("N");
            user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);

            await _repo.Update(user);

            string verifyLink = $"{baseUrl}/verify-email?token={user.EmailVerificationToken}";

            string emailBody = _emailHelper.BuildEmailVerificationEmail(user, verifyLink);

            await _emailHelper.SendEmailAsync(user.Email!, "Verify Your Email", emailBody);

            return (null, user.EmailVerificationToken);
        }

    }
}
