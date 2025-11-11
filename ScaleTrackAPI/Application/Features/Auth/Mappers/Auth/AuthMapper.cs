using ScaleTrackAPI.Application.Features.Auth.DTOs.Login;
using ScaleTrackAPI.Application.Features.Users.DTOs;
using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.Auth.Mappers.Auth.AuthMapper
{
    public static class AuthMapper
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

        /// <summary>
        /// Creates a new RefreshToken entity for storing in the database.
        /// </summary>
        public static RefreshToken ToRefreshToken(this string token, int userId, DateTime expires)
        {
            return new RefreshToken
            {
                Token = token,
                UserId = userId,
                Expires = expires,
                IsRevoked = false,
                IsUsed = false
            };
        }
    }
}
