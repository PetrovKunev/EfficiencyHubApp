using Microsoft.EntityFrameworkCore;
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

        [Precision(18, 2)]
        public decimal AverageTaskCompletionTime { get; set; } 
        public DateTime ReportDate { get; set; }
    }
}
