using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Domain.Entities
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
        
        public int? ChangedBy { get; set; }

        [Required]
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string Changes { get; set; } = null!;

        public int? ApprovedBy { get; set; }
        
        public DateTime? ApprovedAt { get; set; }

        public User? ChangedByUser { get; set; }
        public User? ApprovedByUser { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
