using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Models;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Helpers;
using System.Security.Claims;

namespace ScaleTrackAPI.Services.UserService
{
    public class UserBusinessRules
    {
        private readonly IUserRepository _repo;
        private readonly UserManager<User> _userManager;
        private readonly IValidator<UserRequest> _validator;
        private readonly PasswordHelper _passwordHelper;

        public UserBusinessRules(
            IUserRepository repo,
            UserManager<User> userManager,
            IValidator<UserRequest> validator,
            PasswordHelper passwordHelper)
        {
            _repo = repo;
            _userManager = userManager;
            _validator = validator;
            _passwordHelper = passwordHelper;
        }

        public async Task<(AppError? Error, User? User)> RegisterUserRules(RegisterRequest request)
        {
            if (request == null)
                return (AppError.Validation("Request cannot be null"), null);

            var userRequest = UserMapper.FromRegisterRequest(request);

            var validationResult = _validator.Validate(userRequest);
            if (!validationResult.IsValid)
                return (AppError.Validation(string.Join("; ", validationResult.Errors)), null);

            if (await _repo.GetByEmail(request.Email) != null)
                return (AppError.Conflict($"Email '{request.Email}' already exists"), null);

            var user = UserMapper.ToModel(userRequest, request);
            user.Role = user.Role == 0 ? UserRole.Viewer : user.Role;

            var password = _passwordHelper.WithPepper(request.Password);
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return (AppError.Validation(errors), null);
            }

            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, user.Role.ToString()));

            return (null, user);
        }

        public async Task<(AppError? Error, User? OldUser, User? UpdatedUser)> UpdateUserRoleRules(User user, int roleIndex)
        {
            if (!Enum.IsDefined(typeof(UserRole), roleIndex))
                return (AppError.Validation("Invalid role"), null, null);

            var parsedRole = (UserRole)roleIndex;
            if (user.Role == parsedRole)
                return (AppError.Validation("User already has this role"), null, null);

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
                return (AppError.Conflict("Failed to update user"), null, null);

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
                    return (AppError.Conflict("Failed to delete user"), null);
            }
            else
            {
                await _repo.Delete(user);
            }

            return (null, oldUser);
        }
    }
}
