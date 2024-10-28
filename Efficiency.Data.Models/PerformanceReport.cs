
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfficiencyHub.Data.Models
{
    public class PerformanceReport
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;
        public int CompletedTasks { get; set; }
        public decimal AverageTaskCompletionTime { get; set; } // Време за изпълнение в часове или минути
        public DateTime ReportDate { get; set; }
    }
}
