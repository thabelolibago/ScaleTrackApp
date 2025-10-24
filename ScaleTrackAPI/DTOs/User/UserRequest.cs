namespace ScaleTrackAPI.DTOs.User
{
    public class UserRequest
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UserRole Role { get; set; } = UserRole.Viewer;
    }
}
