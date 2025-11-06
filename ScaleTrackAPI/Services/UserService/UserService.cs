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
using ScaleTrackAPI.Services.Auth;


namespace ScaleTrackAPI.Services.UserService
{
    public class UserService(ITokenService tokenService, IUserRepository repo, UserManager<User> userManager, IValidator<UserRequest> validator, PasswordHelper passwordHelper, UserAuditTrail auditHelper)
    {
        private readonly IUserRepository _repo = repo;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IValidator<UserRequest> _validator = validator;
        private readonly PasswordHelper _passwordHelper = passwordHelper;
        private readonly UserAuditTrail _auditHelper = auditHelper;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<List<UserResponse>> GetAllUsers()
            => (await _repo.GetAll()).Select(UserMapper.ToResponse).ToList();

        public async Task<(UserResponse? Response, AppError? Error)> GetById(int id)
        {
            var u = await _repo.GetById(id);
            if (u == null) return (null, AppError.NotFound(ErrorMessages.Get("User:UserNotFound", id)));
            return (UserMapper.ToResponse(u), null);
        }
        public async Task<(RegisterResponse? Response, AppError? Error)> RegisterUser(RegisterRequest request)
        {
            if (request == null)
                return (null, AppError.Validation(ErrorMessages.Get("Validation:RequestNotNull")));

            // ✅ Validate input
            var validationError = BusinessRules.RegisterValidator.Validate(request);
            if (validationError != null)
                return (null, validationError);

            // ✅ Check if email already exists
            if (await _repo.GetByEmail(request.Email) != null)
                return (null, AppError.Conflict(ErrorMessages.Get("User:EmailAlreadyExists", request.Email)));

            // ✅ Map DTOs
            var userRequest = UserMapper.FromRegisterRequest(request);
            var user = UserMapper.ToModel(userRequest, request);

            var passwordWithPepper = _passwordHelper.WithPepper(request.Password);

            // ✅ Create user in Identity
            var result = await _userManager.CreateAsync(user, passwordWithPepper);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return (null, AppError.Validation(errors));
            }

            // ✅ Assign Viewer role
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, user.Role.ToString()));

            // ✅ Issue tokens
            var (accessToken, refreshToken) = await _tokenService.CreateTokensAsync(user);

            // ✅ Record in audit trail
            var actor = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    }, "Registration"));

            await _auditHelper.RecordCreate(user, actor);

            // ✅ Return response
            var response = new RegisterResponse
            {
                User = UserMapper.ToResponse(user),
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Message = SuccessMessages.Get("User:UserRegistered")
            };

            return (response, null);
        }


        public async Task<(AppError? Error, string Message)> UpdateUserRole(int id, int roleIndex, ClaimsPrincipal userClaims)
        {
            if (!Enum.IsDefined(typeof(UserRole), roleIndex))
                return (AppError.Validation(ErrorMessages.Get("User:InvalidRole")), string.Empty);

            var parsedRole = (UserRole)roleIndex;

            var user = await _repo.GetById(id);
            if (user == null)
                return (AppError.NotFound(ErrorMessages.Get("User:UserNotFound", id)), string.Empty);

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
                return (AppError.Conflict(ErrorMessages.Get("User:FailedToUpdateUser")), string.Empty);

            await _auditHelper.RecordUpdate(oldUser, user, userClaims);

            return (null, SuccessMessages.Get("User:UserUpdated"));
        }

        public async Task<(AppError? Error, string? Message)> DeleteUser(int id, ClaimsPrincipal userClaims)
        {
            var user = await _repo.GetById(id);
            if (user == null) return (AppError.NotFound(ErrorMessages.Get("User:UserNotFound", id)), null);

            var oldUser = new
            {
                user.Id,
                user.Email,
                user.UserName,
                user.FirstName,
                user.LastName,
                Role = user.Role
            };

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
