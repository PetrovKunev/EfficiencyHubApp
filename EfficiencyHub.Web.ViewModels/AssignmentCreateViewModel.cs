using EfficiencyHub.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using static EfficiencyHub.Common.EntityValidationConstants;

namespace EfficiencyHub.Web.ViewModels
{
    public class AssignmentCreateViewModel
    {
        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = "Title length should be between {2} and {1} characters.")]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = "Description length should be between {2} and {1} characters.")]
        public string Description { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        public AssignmentStatus Status { get; set; }
    }
}
