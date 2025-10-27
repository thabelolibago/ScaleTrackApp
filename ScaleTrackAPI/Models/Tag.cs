using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = null!;
        
        public ICollection<IssueTag> IssueTags { get; set; } = new List<IssueTag>();
    }
}
