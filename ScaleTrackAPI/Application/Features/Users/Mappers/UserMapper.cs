

using ScaleTrackAPI.Application.Features.Users.DTOs;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Domain.Enums;

namespace ScaleTrackAPI.Application.Features.Users.Mappers.UserMapper
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
    }
}
