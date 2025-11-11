using ScaleTrackAPI.Application.Features.Auth.RegisterUser.DTOs;
using ScaleTrackAPI.Application.Features.Users.DTOs;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Domain.Enums;

namespace ScaleTrackAPI.Application.Features.Auth.RegisterUser.Mappers
{
    public static class RegisterUserMapper
    {
        public static User ToModel(RegisterUserRequest request)
        {
            return new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                Role = UserRole.Viewer,
                PhoneNumber = request.PhoneNumber,
                Bio = request.Bio,
                ProfilePictureUrl = request.ProfilePictureUrl,
            
            };
        }

        public static UserRequest FromRegisterRequest(RegisterUserRequest request)
        {
            return new UserRequest
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Role = UserRole.Viewer,
            };
        }
    }
}