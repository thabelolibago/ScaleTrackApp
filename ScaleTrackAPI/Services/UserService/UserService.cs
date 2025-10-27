using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Models;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Extensions;
using System.Security.Claims;
using ScaleTrackAPI.Messages;

namespace ScaleTrackAPI.Services
{
    public class UserService(IUserRepository repo, UserManager<User> userManager, IValidator<UserRequest> validator, PasswordHelper passwordHelper)
    {
        private readonly IUserRepository _repo = repo;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IValidator<UserRequest> _validator = validator;
        private readonly PasswordHelper _passwordHelper = passwordHelper;

        public async Task<List<UserResponse>> GetAllUsers()
            => (await _repo.GetAll()).Select(UserMapper.ToResponse).ToList();

        public async Task<(UserResponse? Response, AppError? Error)> GetById(int id)
        {
            var u = await _repo.GetById(id);
            if (u == null) return (null, AppError.NotFound(ErrorMessages.Get("User:UserNotFound", id)));
            return (UserMapper.ToResponse(u), null);
        }

        public async Task<(UserResponse? Response, AppError? Error, string Message)> RegisterUser(RegisterRequest request)
        {
            if (request == null)
                return (null, AppError.Validation(ErrorMessages.Get("Validation:RequestNotNull")), string.Empty);

            var userRequest = UserMapper.FromRegisterRequest(request);

            var validationError = _validator.ToAppError(userRequest);
            if (validationError != null)
                return (null, validationError, string.Empty);

            if (await _repo.GetByEmail(request.Email) != null)
                return (null, AppError.Conflict(ErrorMessages.Get("User:EmailAlreadyExists", request.Email)), string.Empty);

            var user = UserMapper.ToModel(userRequest);
            var passwordWithPepper = _passwordHelper.WithPepper(request.Password);

            var result = await _userManager.CreateAsync(user, passwordWithPepper);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return (null, AppError.Validation(errors), string.Empty);
            }

            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, user.Role.ToString()));

            var response = UserMapper.ToResponse(user);
            var message = SuccessMessages.Get("User:UserRegistered");

            return (response, null, message);
        }


        public async Task<(AppError? Error, string Message)> UpdateUserRole(int id, int roleIndex)
        {
            if (!Enum.IsDefined(typeof(UserRole), roleIndex))
                return (AppError.Validation(ErrorMessages.Get("User:InvalidRole")), string.Empty);

            var parsedRole = (UserRole)roleIndex;

            var user = await _repo.GetById(id);
            if (user == null)
                return (AppError.NotFound(ErrorMessages.Get("User:UserNotFound", id)), string.Empty);

            user.Role = parsedRole;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return (AppError.Conflict(ErrorMessages.Get("User:FailedToUpdateUser")), string.Empty);

            return (null, SuccessMessages.Get("User:UserUpdated"));
        }

        public async Task<(AppError? Error, string? Message)> DeleteUser(int id)
        {
            var user = await _repo.GetById(id);
            if (user == null) return (AppError.NotFound(ErrorMessages.Get("User:UserNotFound", id)), null);

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

            var successMessage = SuccessMessages.Get("User:UserDeleted");
            return (null, successMessage);
        }
    }
}
