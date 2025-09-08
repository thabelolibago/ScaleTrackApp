using ScaleTrackAPI.Models;
using ScaleTrackAPI.DTOs.User;

namespace ScaleTrackAPI.Mappers
{
    public static class UserMapper
    {
        public static UserResponse ToResponse(User user) => new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role
        };

        public static User ToModel(UserRequest request) => new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            Role = request.Role ?? UserRole.Viewer.ToString()
        };

        public static UserRequest FromRegisterRequest(RegisterRequest request) => new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Role = UserRole.Viewer.ToString()
        };
    }
}
