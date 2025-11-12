using ScaleTrackAPI.Application.Features.Auth.Login.DTOs;
using ScaleTrackAPI.Application.Features.Users.DTOs;
using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.Auth.Login.Mappers
{
    public static class LoginMapper
    {
        /// <summary>
        /// Maps a User entity to a LoginResponse including tokens.
        /// Handles parsing of UserRole safely.
        /// </summary>
        public static LoginResponse ToLoginResponse(this User user, string accessToken, string refreshToken)
        {
            return new LoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                User = new UserResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? "",
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    Bio = user.Bio,
                    Role = user.Role
                }
            };
        }
    }
}