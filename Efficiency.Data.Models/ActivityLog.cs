using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EfficiencyHub.Common.Enums;
using static EfficiencyHub.Common.EntityValidationConstants;

namespace EfficiencyHub.Data.Models
{

    public class ActivityLog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        
        [Required]
        public ActionType ActionType { get; set; }

       
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        
        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;
    }
}
