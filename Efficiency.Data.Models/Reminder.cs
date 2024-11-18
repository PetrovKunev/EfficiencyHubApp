using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyHub.Data.Models
{
    public class Reminder
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Message { get; set; } = null!;
        [Required]
        public DateTime ReminderDate { get; set; }
        [Required]
        public Guid AssignmentId { get; set; }
        
        [ForeignKey(nameof(AssignmentId))]
        public Assignment Assignment { get; set; } = null!;
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;
    }
}