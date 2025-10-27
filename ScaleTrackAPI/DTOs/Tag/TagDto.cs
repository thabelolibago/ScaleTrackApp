using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.DTOs.Tag
{
    public class TagDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;
    }
}
