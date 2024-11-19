using static EfficiencyHub.Common.EntityValidationConstants;   
using System.ComponentModel.DataAnnotations;


namespace EfficiencyHub.Web.ViewModels
{
    public class ReminderCreateViewModel
    {
        [Required]
        [MinLength(MessageMinLength)]
        [MaxLength(MessageMaxLength)]
        public string Message { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ReminderDate { get; set; } = DateTime.Now;
        
        [Required]
        public Guid AssignmentId { get; set; }
        
    }
}
