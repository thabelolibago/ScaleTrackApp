using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.Auth.Shared.Mappers.Auth
{
    public static class AuthMapper
    {
      
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
