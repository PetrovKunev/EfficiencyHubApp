using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Web.ViewModels;

public class DashboardService
{
    private readonly IRepository<Project> _projectRepository;
    private readonly IRepository<Assignment> _assignmentRepository;
    private readonly IRepository<ActivityLog> _activityLogRepository;
    private readonly IRepository<Reminder> _reminderRepository;

    public DashboardService(IRepository<Project> projectRepository, IRepository<Assignment> assignmentRepository,
                            IRepository<ActivityLog> activityLogRepository, IRepository<Reminder> reminderRepository)
    {
        _projectRepository = projectRepository;
        _assignmentRepository = assignmentRepository;
        _activityLogRepository = activityLogRepository;
        _reminderRepository = reminderRepository;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        var tasks = await _assignmentRepository.GetAllAsync();
        var activityLogs = await _activityLogRepository.GetAllAsync();
        var reminders = await _reminderRepository.GetAllAsync();

        var activeProjects = projects.Where(p => !p.IsDeleted);
        var activeTasks = tasks.Where(t => !t.IsDeleted);

        return new DashboardViewModel
        {
            ProjectCount = activeProjects.Count(),
            TaskCount = activeTasks.Count(),
            CompletedTaskCount = activeTasks.Count(t => t.Status == AssignmentStatus.Completed),
            RecentActivityLogs = activityLogs.OrderByDescending(a => a.Timestamp).Take(5),
            UpcomingReminders = reminders.Where(r => r.ReminderDate > DateTime.Now).OrderBy(r => r.ReminderDate).Take(5)
        };
    }
}
