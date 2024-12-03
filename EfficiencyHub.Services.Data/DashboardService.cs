using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Web.ViewModels;

namespace EfficiencyHub.Services.Data
{

    public class DashboardService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Assignment> _assignmentRepository;
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<Reminder> _reminderRepository;
        private readonly IRepository<ProjectAssignment> _projectAssignmentRepository;

        public DashboardService(IRepository<Project> projectRepository, IRepository<Assignment> assignmentRepository,
                                IRepository<ActivityLog> activityLogRepository, IRepository<Reminder> reminderRepository, IRepository<ProjectAssignment> projectAssignmentRepository)
        {
            _projectRepository = projectRepository;
            _assignmentRepository = assignmentRepository;
            _activityLogRepository = activityLogRepository;
            _reminderRepository = reminderRepository;
            _projectAssignmentRepository = projectAssignmentRepository;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(Guid userId)
        {
            var projectAssignments = await _projectAssignmentRepository.GetWhereAsync(pa => pa.UserId == userId && !pa.IsDeleted);
            var taskIds = projectAssignments.Select(pa => pa.AssignmentId).Distinct();

            var tasks = await _assignmentRepository.GetWhereAsync(t => taskIds.Contains(t.Id) && !t.IsDeleted);

            var projectIds = projectAssignments.Select(pa => pa.ProjectId).Distinct();
            var projects = await _projectRepository.GetWhereAsync(p => projectIds.Contains(p.Id) && !p.IsDeleted);

            var activityLogs = await _activityLogRepository.GetWhereAsync(a => a.UserId == userId);
            var reminders = await _reminderRepository.GetWhereAsync(r => r.UserId == userId && !r.IsDeleted);

            return new DashboardViewModel
            {
                ProjectCount = projects.Count(),
                TaskCount = tasks.Count(),
                CompletedTaskCount = tasks.Count(t => t.Status == AssignmentStatus.Completed),
                RecentActivityLogs = activityLogs.OrderByDescending(a => a.Timestamp).Take(5),
                UpcomingReminders = reminders
                                    .Where(r => r.ReminderDate > DateTime.Now)
                                    .OrderBy(r => r.ReminderDate)
                                    .Take(5)
            };
        }

    }
}
