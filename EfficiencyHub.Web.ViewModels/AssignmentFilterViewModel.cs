using EfficiencyHub.Common.Enums;

namespace EfficiencyHub.Web.ViewModels
{
    public class AssignmentFilterViewModel
    {
        public string? Title { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public AssignmentStatus? Status { get; set; }
    }
}
