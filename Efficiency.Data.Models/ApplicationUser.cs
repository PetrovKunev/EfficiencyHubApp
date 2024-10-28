using Efficiency.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace EfficiencyHub.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid();
        }

        // Проекти, създадени от потребителя
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

        // Напомняния, свързани с потребителя
        public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();

        // Логове на дейности на потребителя
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

        // Назначенията на задачи и роли на потребителя
        public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();

        // Отчети за производителност на потребителя
        public virtual ICollection<PerformanceReport> PerformanceReports { get; set; } = new List<PerformanceReport>();
    }
}
