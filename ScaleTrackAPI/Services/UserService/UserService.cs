using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Models;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Extensions;
using System.Security.Claims;

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

        public async Task<UserResponse?> GetById(int id)
            => (await _repo.GetById(id)) is User u ? UserMapper.ToResponse(u) : null;

        public async Task<(UserResponse?, AppError?)> RegisterUser(RegisterRequest request)
        {
            if (request == null)
                return (null, AppError.Validation("Request cannot be null."));

            var userRequest = UserMapper.FromRegisterRequest(request);

            var validationError = _validator.ToAppError(userRequest);
            if (validationError != null) return (null, validationError);

            if (await _repo.GetByEmail(request.Email) != null)
                return (null, AppError.Conflict($"User with email '{request.Email}' already exists."));

            var user = UserMapper.ToModel(userRequest);
            var passwordWithPepper = _passwordHelper.WithPepper(request.Password);

            var result = await _userManager.CreateAsync(user, passwordWithPepper);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return (null, AppError.Validation(errors));
            }

            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, user.Role));

            return (UserMapper.ToResponse(user), null);
        }

        public async Task<AppError?> UpdateUserRole(int id, string role)
        {
            if (!Enum.TryParse<UserRole>(role, true, out var parsedRole))
                return AppError.Validation("Invalid role");

            var user = await _repo.GetById(id);
            if (user == null) return AppError.NotFound($"User with id {id} not found.");

            user.Role = parsedRole.ToString();
            await _userManager.UpdateAsync(user);

            return null;
        }

        public async Task<AppError?> DeleteUser(int id)
        {
            var user = await _repo.GetById(id);
            if (user == null) return AppError.NotFound($"User with id {id} not found.");

            var identityUser = await _userManager.FindByIdAsync(id.ToString());
            if (identityUser != null)
            {
                var res = await _userManager.DeleteAsync(identityUser);
                if (!res.Succeeded) return AppError.Conflict("Failed to delete user.");
            }
            else
            {
                await _repo.Delete(user);
            }

            return null;
        }
    }
}
