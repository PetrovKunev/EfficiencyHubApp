using EfficiencyHub.Data.Models;

namespace EfficiencyHub.Web.ViewModels
{
    public class DashboardViewModel
    {
        public int ProjectCount { get; set; }
        public int TaskCount { get; set; }
        public int CompletedTaskCount { get; set; }
        public IEnumerable<ActivityLog> RecentActivityLogs { get; set; } = new List<ActivityLog>();
        public IEnumerable<Reminder> UpcomingReminders { get; set; } = new List<Reminder>();
    }
}
