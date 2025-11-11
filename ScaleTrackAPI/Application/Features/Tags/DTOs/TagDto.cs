using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Application.Features.Tags.DTOs
{
    public class TagDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;
    }
}
