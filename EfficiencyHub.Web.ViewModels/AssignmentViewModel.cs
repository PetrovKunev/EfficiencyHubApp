using EfficiencyHub.Common.Enums;

namespace EfficiencyHub.Web.ViewModels
{
    public class AssignmentViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public AssignmentStatus Status { get; set; }

        public bool IsDeleted { get; set; }
    }
}
