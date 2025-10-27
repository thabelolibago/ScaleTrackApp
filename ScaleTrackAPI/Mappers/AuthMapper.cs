using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.Models;
using System;

namespace ScaleTrackAPI.Mappers
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
