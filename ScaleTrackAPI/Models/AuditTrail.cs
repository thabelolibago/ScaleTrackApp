using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Models
{
    public class AuditTrail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; } = null!;

        [Required]
        public int EntityId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = null!;
        
        [Required]
        [MaxLength(100)]
        public int ChangedBy { get; set; }

        [Required]
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(1000)]
        public string Changes { get; set; } = null!;

        public int? ApprovedBy { get; set; }
        
        public DateTime? ApprovedAt { get; set; }

        public User? ChangedByUser { get; set; }
        public User? ApprovedByUser { get; set; }
    }
}
