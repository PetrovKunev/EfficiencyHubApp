using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfficiencyHub.Services.Data
{
    public class ActivityLogService
    {
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly ILogger<ActivityLogService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Assignment> _assignmentRepository;
        private readonly IRepository<Reminder> _reminderRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<ProjectAssignment> _projectAssignmentRepository;

        public ActivityLogService(IRepository<ActivityLog> activityLogRepository, ILogger<ActivityLogService> logger, UserManager<ApplicationUser> userManager, IRepository<Assignment> assignmentRepository, IRepository<Reminder> reminderRepository, IRepository<Project> projectRepository, IRepository<ProjectAssignment> projectAssignmentRepository)
        {
            _activityLogRepository = activityLogRepository;
            _logger = logger;
            _userManager = userManager;
            _assignmentRepository = assignmentRepository;
            _reminderRepository = reminderRepository;
            _projectRepository = projectRepository;
            _projectAssignmentRepository = projectAssignmentRepository;
        }

        public async Task<IEnumerable<ActivityLogViewModel>> GetLastUserActionsAsync(Guid userId, int count = 10)
        {
            try
            {
                var logs = await _activityLogRepository.GetWhereAsync(a => a.UserId == userId);
                return logs.OrderByDescending(log => log.Timestamp)
                           .Take(count)
                           .Select(log => new ActivityLogViewModel
                           {
                               Timestamp = log.Timestamp,
                               ActionType = log.ActionType.ToString(),
                               Description = log.Description
                           }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user activity logs.");
                throw;
            }
        }

        public async Task LogActionAsync(Guid userId, ActionType actionType, string description, Guid? relatedId = null, string? relatedEntityType = null)
        {
            try
            {
                string detailedDescription;

                // Определяне на иконите и стиловете според типа действие
                string icon = actionType switch
                {
                    ActionType.Created => "<span class='text-success'>✅</span>",
                    ActionType.Deleted => "<span class='text-danger'>❌</span>",
                    ActionType.Updated => "<span class='text-primary'>🔄</span>",
                    _ => "<span class='text-secondary'>ℹ️</span>" // За други действия
                };

                // Определяне на описанието според типа свързан обект
                switch (relatedEntityType)
                {
                    case "Assignment":
                        if (relatedId.HasValue)
                        {
                            var assignment = await _assignmentRepository.GetByIdAsync(relatedId.Value);
                            if (assignment != null)
                            {
                                // Зареждане на проекта, към който е свързана задачата
                                var projectAssignment = await _projectAssignmentRepository
                                    .GetQueryableWhere(pa => pa.AssignmentId == assignment.Id)
                                    .Include(pa => pa.Project) // Ensure the Project is included in the query
                                    .FirstOrDefaultAsync();

                                var projectName = projectAssignment?.Project?.Name ?? "Project not found";
                                detailedDescription = $"{icon} {actionType} assignment: '{assignment.Title}' (part of project: '{projectName}')";
                            }
                            else
                            {
                                detailedDescription = $"{icon} {actionType} assignment (details not found).";
                            }
                        }
                        else
                        {
                            detailedDescription = $"{icon} {actionType} assignment (details not found).";
                        }
                        break;

                    case "Reminder":
                        if (relatedId.HasValue)
                        {
                            var reminder = await _reminderRepository.GetByIdAsync(relatedId.Value);
                            if (reminder != null && reminder.Assignment != null)
                            {
                                detailedDescription = $"{icon} {actionType} reminder: '{reminder.Message}' (linked to assignment: '{reminder.Assignment.Title}')";
                            }
                            else
                            {
                                detailedDescription = $"{icon} {actionType} reminder (details not found).";
                            }
                        }
                        else
                        {
                            detailedDescription = $"{icon} {actionType} reminder (details not found).";
                        }
                        break;

                    case "Project":
                        if (relatedId.HasValue)
                        {
                            var project = await _projectRepository.GetByIdAsync(relatedId.Value);
                            if (project != null)
                            {
                                detailedDescription = $"{icon} {actionType} project: '{project.Name}'";
                            }
                            else
                            {
                                detailedDescription = $"{icon} {actionType} project (details not found).";
                            }
                        }
                        else
                        {
                            detailedDescription = $"{icon} {actionType} project (details not found).";
                        }
                        break;

                    default:
                        detailedDescription = $"{icon} {actionType} unknown entity.";
                        break;
                }

                var log = new ActivityLog
                {
                    UserId = userId,
                    ActionType = actionType,
                    Description = detailedDescription,
                    Timestamp = DateTime.Now
                };

                await _activityLogRepository.AddAsync(log);
                _logger.LogInformation("Successfully logged action: {ActionType} by User: {UserId}", actionType, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging action: {ActionType} for User: {UserId}", actionType, userId);
                throw;
            }
        }

    }
}