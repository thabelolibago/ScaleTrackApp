using ScaleTrackAPI.Models;
using ScaleTrackAPI.DTOs.User;
using System;

namespace ScaleTrackAPI.Mappers
{
    public static class UserMapper
    {
        public static UserResponse ToResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Bio = user.Bio,
                Role = user.Role
            };
        }
        public static User ToModel(RegisterRequest request)
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
                ProfilePictureUrl = request.ProfilePictureUrl
            };
        }

        public static UserRequest FromRegisterRequest(RegisterRequest request)
        {
            return new UserRequest
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Role = UserRole.Viewer
            };
        }
    }
}
