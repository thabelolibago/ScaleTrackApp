using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.DTOs.Tag
{
    public class TagRequest
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
