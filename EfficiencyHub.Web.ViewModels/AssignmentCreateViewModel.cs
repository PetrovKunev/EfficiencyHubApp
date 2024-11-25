using EfficiencyHub.Common;
using EfficiencyHub.Common.Enums;
using System.ComponentModel.DataAnnotations;
using static EfficiencyHub.Common.EntityValidationConstants;

namespace EfficiencyHub.Web.ViewModels
{
    public class AssignmentCreateViewModel
    {
        public Guid ProjectId { get; set; }

        [Required]
        [MinLength(NameMinLength, ErrorMessage = ValidationMessages.NameLength)]
        [MaxLength(NameMaxLength, ErrorMessage = ValidationMessages.NameLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MinLength(DescriptionMinLength, ErrorMessage = ValidationMessages.DescriptionLength)]
        [MaxLength(DescriptionMaxLength, ErrorMessage = ValidationMessages.DescriptionLength)]
        public string Description { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        public AssignmentStatus Status { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? CompletedDate { get; set; }
    }
}
