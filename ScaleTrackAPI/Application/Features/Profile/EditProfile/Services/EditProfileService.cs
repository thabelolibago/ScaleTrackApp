using System.Security.Claims;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Features.Auth.DTOs.Password;
using ScaleTrackAPI.Application.Features.Auth.Password.ChangePassword.Services.ChangePasswordService;
using ScaleTrackAPI.Application.Features.Profile.EditProfile.BusinessRules.EditProfileAuditTrail;
using ScaleTrackAPI.Application.Features.Profile.EditProfile.DTOs;
using ScaleTrackAPI.Application.Features.Profile.EditProfile.Mappers.EditProfileMapper;
using ScaleTrackAPI.Application.Features.Users.Mappers.UserMapper;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IUserRepository;

namespace ScaleTrackAPI.Application.Features.Profile.EditProfile.Services.EditProfileService
{
    public class EditProfileService
    {
        private readonly IUserRepository _userRepo;
        private readonly EditProfileAuditTrail _audit;
        private readonly ChangePasswordService _passwordService;

        public EditProfileService(
            IUserRepository userRepo,
            EditProfileAuditTrail audit,
            ChangePasswordService passwordService)
        {
            _userRepo = userRepo;
            _audit = audit;
            _passwordService = passwordService;
        }

        public async Task<(EditProfileResponse? Response, AppError? Error)> EditProfileAsync(
            int userId, EditProfileRequest request, ClaimsPrincipal actor)
        {
            var user = await _userRepo.GetById(userId);
            if (user == null)
                return (null, AppError.NotFound("User not found."));

            var oldUser = new User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Email = user.Email
            };

            // Handle password change
            if (!string.IsNullOrWhiteSpace(request.CurrentPassword) &&
                !string.IsNullOrWhiteSpace(request.NewPassword) &&
                !string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                await _passwordService.ChangePasswordAsync(userId, new ChangePasswordRequest
                {
                    CurrentPassword = request.CurrentPassword,
                    NewPassword = request.NewPassword,
                    ConfirmPassword = request.ConfirmPassword
                }, actor);
            }

            // Apply profile updates using mapper
            bool updated = user.ApplyProfileChanges(request);

            if (updated)
            {
                await _userRepo.Update(user);
                await _audit.RecordUpdate(oldUser, user, actor);
            }

            return (new EditProfileResponse
            {
                User = UserMapper.ToResponse(user),
                Message = updated ? "Profile updated successfully." : null
            }, null);
        }
    }
}
