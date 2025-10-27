using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Path { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Icon { get; set; } = null!;

        [Required]
        public string Roles { get; set; } = null!; 
    }
}
