namespace ScaleTrackAPI.DTOs.User
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public string ProfilePictureUrl { get; set; } = null!;
        public UserRole Role { get; set; } = UserRole.Viewer;
    }
}
