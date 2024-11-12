using EfficiencyHub.Common.Enums;

namespace EfficiencyHub.Web.ViewModels
{
    public class ProjectViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ProjectRole Role { get; set; }
        public bool IsDeleted { get; set; }
    }
}
