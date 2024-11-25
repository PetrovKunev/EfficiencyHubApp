using Microsoft.AspNetCore.Identity;

namespace EfficiencyHub.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid();
        }
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
        public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();
        public virtual ICollection<PerformanceReport> PerformanceReports { get; set; } = new List<PerformanceReport>();
    }
}
