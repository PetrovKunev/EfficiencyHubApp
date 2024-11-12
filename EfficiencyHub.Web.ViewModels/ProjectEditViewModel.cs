using EfficiencyHub.Common.Enums;
using EfficiencyHub.Common;
using System.ComponentModel.DataAnnotations;

using static EfficiencyHub.Common.EntityValidationConstants;

namespace EfficiencyHub.Web.ViewModels
{
    public class ProjectEditViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(NameMinLength, ErrorMessage = ValidationMessages.NameLength)]
        [MaxLength(NameMaxLength, ErrorMessage = ValidationMessages.NameLength)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(DescriptionMinLength, ErrorMessage = ValidationMessages.DescriptionLength)]
        [MaxLength(DescriptionMaxLength, ErrorMessage = ValidationMessages.DescriptionLength)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DateGreaterThan("StartDate", ErrorMessage = ValidationMessages.EndDateAfterStartDate)]
        public DateTime EndDate { get; set; }

        [Required]
        public ProjectRole Role { get; set; }

        public List<ProjectRole> AvailableRoles { get; set; } = new List<ProjectRole>();
    }
}
