using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Mappers
{
    public static class PasswordResetMapper
    {
        public static PasswordResetToken ToEntity(int userId, string token, DateTime expiration)
        {
            return new PasswordResetToken
            {
                UserId = userId,
                Token = token,
                Expiration = expiration
            };
        }

        public static ResetPasswordResponse ToResponse(bool success, string message)
        {
            return new ResetPasswordResponse
            {
                Success = success,
                Message = message
            };
        }
    }
}
