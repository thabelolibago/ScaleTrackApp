using ScaleTrackAPI.Models;
using ScaleTrackAPI.DTOs.User;
using System;

namespace ScaleTrackAPI.Mappers
{
    public static class UserMapper
    {
        public static UserResponse ToResponse(User user)
        {
            var role = Enum.TryParse<UserRole>(user.Role, out var parsedRole)
                       ? parsedRole
                       : UserRole.Viewer;

            return new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Role = role
            };
        }

        public static User ToModel(UserRequest request)
        {
            return new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                Role = request.Role.ToString() 
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
