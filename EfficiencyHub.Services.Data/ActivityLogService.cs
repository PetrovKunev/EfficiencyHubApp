using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
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



        public ActivityLogService(IRepository<ActivityLog> activityLogRepository, ILogger<ActivityLogService> logger, UserManager<ApplicationUser> userManager, IRepository<Assignment> assignmentRepository, IRepository<Reminder> reminderRepository)
        {
            _activityLogRepository = activityLogRepository;
            _logger = logger;
            _userManager = userManager;
            _assignmentRepository = assignmentRepository;
            _reminderRepository = reminderRepository;
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
                string detailedDescription = description;

                // Ако е свързано с напомняне или задача, извлечете името
                if (relatedId.HasValue && !string.IsNullOrEmpty(relatedEntityType))
                {
                    if (relatedEntityType == "Assignment")
                    {
                        var assignment = await _assignmentRepository.GetByIdAsync(relatedId.Value);
                        if (assignment != null)
                        {
                            detailedDescription += $" for assignment '{assignment.Title}'";
                        }
                    }
                    else if (relatedEntityType == "Reminder")
                    {
                        var reminder = await _reminderRepository.GetByIdAsync(relatedId.Value);
                        if (reminder != null && reminder.Assignment != null)
                        {
                            detailedDescription += $" for reminder with message '{reminder.Message}' (assignment: '{reminder.Assignment.Title}')";
                        }
                    }
                }

                var log = new ActivityLog
                {
                    UserId = userId,
                    ActionType = actionType,
                    Description = detailedDescription,
                    Timestamp = DateTime.UtcNow
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