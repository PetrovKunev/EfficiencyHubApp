using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyHub.Data.Models
{
    [PrimaryKey(nameof(ProjectId), nameof(AssignmentId))]
    public class ProjectAssignment
    {
        // Foreign key към Project
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        // Foreign key към Assignment
        [Required]
        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;

        [Required]
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

    }
}
