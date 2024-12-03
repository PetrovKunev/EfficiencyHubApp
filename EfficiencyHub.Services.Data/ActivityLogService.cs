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

        public async Task<int> GetTotalLogsAsync(Guid userId)
        {
            return await _activityLogRepository.GetQueryableWhere(a => a.UserId == userId).CountAsync();
        }

        public async Task<IEnumerable<ActivityLogViewModel>> GetPagedUserActionsAsync(Guid userId, int pageNumber, int pageSize)
        {
            try
            {
                var logs = await _activityLogRepository.GetQueryableWhere(a => a.UserId == userId)
                    .OrderByDescending(log => log.Timestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return logs.Select(log => new ActivityLogViewModel
                {
                    Timestamp = log.Timestamp,
                    ActionType = log.ActionType.ToString(),
                    Description = log.Description
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged user activity logs.");
                throw;
            }
        }



        public async Task LogActionAsync(Guid userId, ActionType actionType, string description, Guid? relatedId = null, string? relatedEntityType = null)
        {
            try
            {
                string detailedDescription;

                string icon = actionType switch
                {
                    ActionType.Created => "<span class='text-success'>✅</span>",
                    ActionType.Deleted => "<span class='text-danger'>❌</span>",
                    ActionType.Updated => "<span class='text-primary'>🔄</span>",
                    _ => "<span class='text-secondary'>ℹ️</span>"
                };

                switch (relatedEntityType)
                {
                    case "Assignment":
                        if (relatedId.HasValue)
                        {
                            var assignment = await _assignmentRepository
                                .GetQueryableWhere(a => a.Id == relatedId.Value)
                                .Include(a => a.ProjectAssignments.Where(pa => !pa.IsDeleted))
                                .ThenInclude(pa => pa.Project)
                                .FirstOrDefaultAsync();

                            if (assignment != null)
                            {
                                var projectName = assignment.ProjectAssignments
                                    .FirstOrDefault()?.Project?.Name ?? "Project not found";

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
                            var reminder = await _reminderRepository
                                .GetQueryableWhere(r => r.Id == relatedId.Value)
                                .Include(r => r.Assignment)
                                .FirstOrDefaultAsync();

                            if (reminder != null)
                            {
                                if (reminder.Assignment != null)
                                {
                                    detailedDescription = $"{icon} {actionType} reminder: '{reminder.Message}' (linked to assignment: '{reminder.Assignment.Title}')";
                                }
                                else
                                {
                                    detailedDescription = $"{icon} {actionType} reminder: '{reminder.Message}' (assignment details not found).";
                                }
                            }
                            else if (reminder != null)
                            {
                                detailedDescription = $"{icon} {actionType} reminder: '{reminder.Message}' (assignment not found)";
                            }
                            else
                            {
                                detailedDescription = $"{icon} {actionType} reminder (details not found or already deleted).";
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

        public async Task<(IEnumerable<ActivityLogViewModel> Items, int TotalCount)> SearchActivityLogsAsync(Guid userId, string? searchTerm, int pageNumber, int pageSize)
        {
            try
            {
                var query = _activityLogRepository.GetQueryableWhere(log => log.UserId == userId);

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var lowerSearchTerm = searchTerm.ToLower();
                    query = query.Where(log => log.Description.ToLower().Contains(lowerSearchTerm));
                }

                var totalCount = await query.CountAsync();

                var logs = await query
                    .OrderByDescending(log => log.Timestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();
                
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var lowerSearchTerm = searchTerm.ToLower();
                    logs = logs
                        .Where(log =>
                            log.ActionType.ToString().ToLower().Contains(lowerSearchTerm) ||
                            log.Description.ToLower().Contains(lowerSearchTerm))
                        .ToList();
                }

                var logViewModels = logs.Select(log => new ActivityLogViewModel
                {
                    Timestamp = log.Timestamp,
                    ActionType = log.ActionType.ToString(),
                    Description = log.Description
                });

                return (logViewModels, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching activity logs.");
                throw;
            }
        }
    }
}