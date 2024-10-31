using EfficiencyHub.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EfficiencyHub.Common.EntityValidationConstants;

namespace EfficiencyHub.Data.Models
{
    // за доработка
    public class ActivityLog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Дата и час на действието
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Тип на действието ("Created", "Updated", "Deleted")
        [Required]
        public ActionType ActionType { get; set; }

        // Идентификатор на потребителя, извършил действието
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        // Описание на действието (напр., "Project Created" или "Task Deleted")
        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;
    }
}
