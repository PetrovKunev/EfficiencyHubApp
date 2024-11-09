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
        [StringLength(100, ErrorMessage = "The name must be between 3 and 100 characters.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "The description must be between 10 and 500 characters.", MinimumLength = 10)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        //[Required]
        //[DataType(DataType.Date)]
        //[DateGreaterThan("StartDate", ErrorMessage = "End date must be after the start date.")]
        //public DateTime EndDate { get; set; }
    }
}
