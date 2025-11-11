using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Application.Features.Tags.DTOs
{
    public class TagRequest
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
