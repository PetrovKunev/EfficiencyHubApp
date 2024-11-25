using EfficiencyHub.Common.Enums;
using System.ComponentModel.DataAnnotations;
using static EfficiencyHub.Common.EntityValidationConstants;

namespace EfficiencyHub.Data.Models
{
    public class Assignment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Title { get; set; } = null!;
        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;
        [Required]
        public DateTime DueDate { get; set; }
        
        [Required]
        public AssignmentStatus Status { get; set; }

        public ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();

        public bool IsDeleted { get; set; }

        // Нови полета
        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? CompletedDate { get; set; }
    }
}
