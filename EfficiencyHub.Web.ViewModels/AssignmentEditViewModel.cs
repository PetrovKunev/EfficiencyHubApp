using EfficiencyHub.Common.Enums;
using static EfficiencyHub.Common.EntityValidationConstants;
using System.ComponentModel.DataAnnotations;
using EfficiencyHub.Common;

namespace EfficiencyHub.Web.ViewModels
{
    public class AssignmentEditViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(NameMinLength, ErrorMessage = ValidationMessages.NameLength)]
        [MaxLength(NameMaxLength, ErrorMessage = ValidationMessages.NameLength)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(DescriptionMinLength, ErrorMessage = ValidationMessages.DescriptionLength)]
        [MaxLength(DescriptionMaxLength, ErrorMessage = ValidationMessages.DescriptionLength)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Required]
        public AssignmentStatus Status { get; set; }

        public Guid ProjectId { get; set; }


        [Display(Name = "Completed Date")]
        [DataType(DataType.DateTime)]
        public DateTime? CompletedDate { get; set; }
    }
}
