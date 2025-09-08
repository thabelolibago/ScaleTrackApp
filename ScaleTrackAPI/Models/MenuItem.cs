using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Path { get; set; } = null!;
        public string Icon { get; set; } = null!;
        public string Roles { get; set; } = null!; 
    }
}
