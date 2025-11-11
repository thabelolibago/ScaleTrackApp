using ScaleTrackAPI.Application.Features.Auth.DTOs.Password;
using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.Auth.Mappers.Password.PasswordResetMapper
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
