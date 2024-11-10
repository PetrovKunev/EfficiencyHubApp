using EfficiencyHub.Common;
using EfficiencyHub.Common.Enums;
using System.ComponentModel.DataAnnotations;


namespace EfficiencyHub.Web.ViewModels
{
    
public class ProjectCreateViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = ValidationMessages.NameLength, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500, ErrorMessage = ValidationMessages.DescriptionLength, MinimumLength = 10)]
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

        public IEnumerable<ProjectRole> AvailableRoles { get; set; } = Enum.GetValues(typeof(ProjectRole)).Cast<ProjectRole>();
    }
}
