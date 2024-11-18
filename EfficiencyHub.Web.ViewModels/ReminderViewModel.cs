using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyHub.Web.ViewModels
{
    public class ReminderViewModel
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ReminderDate { get; set; }
        public Guid AssignmentId { get; set; }
        public string AssignmentTitle { get; set; } = string.Empty;
        public string AssignmentName { get; set; } = string.Empty;  
    }
}
