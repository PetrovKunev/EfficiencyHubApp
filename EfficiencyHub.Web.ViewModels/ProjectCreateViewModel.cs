using EfficiencyHub.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyHub.Web.ViewModels
{
    //за доработка валидации
    public class ProjectCreateViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = ValidationMessages.NameLength, MinimumLength = 3)]
        public required string Name { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = ValidationMessages.DescriptionLength, MinimumLength = 10)]
        public required string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DateGreaterThan("StartDate", ErrorMessage = ValidationMessages.EndDateAfterStartDate)]
        public DateTime EndDate { get; set; }
    }
}
