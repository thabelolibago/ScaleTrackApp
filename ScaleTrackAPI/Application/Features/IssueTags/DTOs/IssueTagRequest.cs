using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Application.Features.IssueTags.DTOs
{
    public class IssueTagRequest
    {
        [Required]
        public int TagId { get; set; }
    }
}
