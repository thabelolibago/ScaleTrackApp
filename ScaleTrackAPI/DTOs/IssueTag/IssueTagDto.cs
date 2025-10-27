using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.DTOs.IssueTag
{
    public class IssueTagDto
    {
        [Required]
        public int IssueId { get; set; }

        [Required]
        public int TagId { get; set; }
    }
}
