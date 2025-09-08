namespace ScaleTrackAPI.DTOs.Menu
{
    public class MenuItemDto
    {
        public string Name { get; set; } = null!; 
        public string Path { get; set; } = null!; 
        public string Icon { get; set; } = null!; 
        public string[] Roles { get; set; } = null!; 
    }
}
